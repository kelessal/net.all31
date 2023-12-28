using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Net.Extensions;
namespace Net.Reflection
{
    public class TypeInfo
    {
        static ConcurrentDictionary<Type, TypeInfo> _objectInfos = new ConcurrentDictionary<Type, TypeInfo>();

        public Type Type { get; private set; }
        public string Name { get; private set; }
        public string DashedName { get; private set; }
        public TypeKind Kind { get; private set; }
        public TypeInfo ElementTypeInfo { get; private set; }

        private readonly HashSet<Type> BaseInterfaces = new HashSet<Type>();
        public IEnumerable<Type> GetBaseInterfaces() => this.BaseInterfaces.AsEnumerable();
        public bool ContainsBaseInterface(Type type) => this.BaseInterfaces.Contains(type);
        public bool IsPrimitiveCollection => this.ElementTypeInfo != null && this.ElementTypeInfo.Kind == TypeKind.Primitive;
        private readonly Dictionary<string, TypePropertyInfo> _allProperties = new Dictionary<string, TypePropertyInfo>();
        private readonly Dictionary<string, TypePropertyInfo> _camelCaseProperties = new Dictionary<string, TypePropertyInfo>();

        public TypePropertyInfo GetPropertyByPath(string path)
        {
            var currentPath = path.TrimThenBy(".");
            var thenPath = path.TrimLeftBy(".");
            if (thenPath.IsEmpty() || thenPath == currentPath) return this[currentPath];
            var currentProp = this[currentPath];
            if (currentProp.IsNull()) return null;
            switch (currentProp.Kind)
            {
                case TypeKind.Unknown:
                    return null;
                case TypeKind.Primitive:
                    return null;
                case TypeKind.Complex:
                    return currentProp.Type.GetInfo().GetPropertyByPath(thenPath);
                case TypeKind.Collection:
                    return currentProp.ElementTypeInfo.GetPropertyByPath(thenPath);
                default:
                    return null;
            }
        }


        public IEnumerable<TypePropertyInfo> GetPropertiesByAttribute<T>()
            where T : Attribute
        => this._allProperties.Values.Where(p => p.HasAttribute<T>());
        public IEnumerable<TypePropertyInfo> GetAllProperties() => this._allProperties.Values.AsEnumerable();
        public TypePropertyInfo this[string propName]
        {
            get
            {
                return this._allProperties.GetSafeValue(propName) ??
                    this._camelCaseProperties.GetSafeValue(propName);
            }
        }
        public bool HasProperty(string name) =>
            this._allProperties.ContainsKey(name) || this._camelCaseProperties.ContainsKey(name);
        public int PropertySize => this._allProperties.Count;
        private TypeInfo()
        {

        }
        internal static TypeInfo GetTypeInfo(Type type, Dictionary<Type, TypeInfo> workingInfos = null)
        {
            if (_objectInfos.ContainsKey(type)) return _objectInfos[type];
            if (workingInfos != null && workingInfos.ContainsKey(type))
                return workingInfos[type];
            lock (type)
            {
                if (_objectInfos.ContainsKey(type)) return _objectInfos[type];
                workingInfos = workingInfos ?? new Dictionary<Type, TypeInfo>();
                var info = new TypeInfo();
                info.Type = type;
                info.Name = type.Name;
                info.DashedName = type.IsInterface && type.Name.StartsWith("I") ?
                    type.Name.Substring(1).ToDashCase() : type.Name.ToDashCase();
                workingInfos.Add(type, info);
                info.Kind = info.Type.GetTypeKind();
                if (info.Kind == TypeKind.Complex)
                    info.ParseProperties(workingInfos);
                else if (info.Kind == TypeKind.Collection)
                    info.ElementTypeInfo = GetTypeInfo(info.Type.GetCollectionElementType(), workingInfos);
                _objectInfos[type] = info;
                void IncludeInterfaces(Type currentType)
                {
                    foreach (var subType in currentType.GetInterfaces())
                    {
                        if (info.BaseInterfaces.Contains(subType)) continue;
                        info.BaseInterfaces.Add(subType);
                        IncludeInterfaces(subType);
                    }
                }
                IncludeInterfaces(type);
                return info;
            }

        }



        private void ParseProperties(Dictionary<Type, TypeInfo> workingTypes)
        {
            foreach (var propInfo in this.Type.FindProperties())
            {
               var prop = TypePropertyInfo.Create(this,propInfo, workingTypes);
                this._allProperties[propInfo.Name] = prop;
                this._camelCaseProperties[prop.CamelName] = prop;
            }
        }

        public T GetAttribute<T>()
            where T : Attribute
        {
            return this.Type.GetCustomAttribute<T>();
        }
        public IEnumerable<T> GetAttributes<T>()
            where T : Attribute
        {
            return this.Type.GetCustomAttributes<T>();
        }
        public override string ToString()
            => this.Type.ToString();


    }
}
