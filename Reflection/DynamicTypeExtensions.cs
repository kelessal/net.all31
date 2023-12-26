using Net.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Net.Reflection
{
    public static class DynamicTypeExtensions
    {
        public static ExpandoObject ConvertToExpando(this object obj, Func<TypePropertyInfo, bool> filter = null)
        {
            if (obj == null) return null;
            if (obj is ExpandoObject expandoObj) return expandoObj;
            IDictionary<string, object> result = new ExpandoObject();
            IDictionary<string, object> nameValues = new Dictionary<string, object>();
            if (obj is Dictionary<string, object>)
                nameValues = obj as IDictionary<string, object>;
            else
            {
                var info = obj.GetType().GetInfo();
                if (info.Kind != TypeKind.Complex) return null;
                var props = info.GetAllProperties();
                if (filter != null)
                    props.Where(p => filter(p));
                props.Foreach(p => nameValues.Add(p.Name, p.GetValue(obj)));
            }

            foreach (var prop in nameValues)
            {
                var name = prop.Key;
                var value = prop.Value;
                if (value == null)
                {
                    result.Add(name, null);
                    continue;
                }
                var info = value.GetType().GetInfo();
                if (value is IDictionary<string, object>)
                {
                    result.Add(name, value.ConvertToExpando(filter));
                }
                switch (info.Kind)
                {
                    case TypeKind.Unknown:
                    case TypeKind.Primitive:
                        result.Add(name, value);
                        continue;
                    case TypeKind.Complex:
                        result.Add(name, value.ConvertToExpando(filter));
                        continue;
                    case TypeKind.Collection:
                        if (info.IsPrimitiveCollection)
                        {
                            result.Add(name, value);
                            continue;
                        }
                        var list = new List<object>();
                        foreach (var subValue in value as IEnumerable)
                        {
                            list.Add(value.ConvertToExpando(filter));
                        }
                        result.Add(name, list);
                        continue;
                }
            }

            return (ExpandoObject)result;
        }

       
    }
}
