using Net.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Net.Json
{
    public class DynamicContractResolver : CamelCasePropertyNamesContractResolver
    {
        public bool LowerFirstLetter { get; set; } = true;
        public bool MongoIdConversion { get; set; } = true;
        protected override string ResolveDictionaryKey(string dictionaryKey)
        {
            dictionaryKey = LowerFirstLetter ? dictionaryKey.ToLowerFirstLetter(isInvariant:true) : dictionaryKey;
            if (MongoIdConversion && dictionaryKey == "_id") return "id";
            return dictionaryKey;
        }
     
        protected override string ResolvePropertyName(string propertyName)
        {
            return LowerFirstLetter
                ?propertyName.ToLowerFirstLetter(isInvariant:true)
                :propertyName;
        }
        protected override JsonProperty CreateProperty(MemberInfo member
                                                 , MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (MongoIdConversion &&  prop.PropertyName == "_id")
            {
                prop.PropertyName = "id";
            } else
            {
                prop.PropertyName = LowerFirstLetter
                    ?member.Name.ToLowerFirstLetter(isInvariant:true)
                    :member.Name;
            }
            if (member.ReflectedType.IsInterface && !member.ReflectedType.IsCollectionType())
            {
                prop.Converter = ConcreteConverter.Default;
                prop.Writable = true;
                return prop;
            }
            if (prop.Readable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }
            if (member.ReflectedType.IsAssignableTo<Expression>())
                prop.Converter = ExpressionJsonConverter.Default;
            return prop;
        }
    }
    
}

