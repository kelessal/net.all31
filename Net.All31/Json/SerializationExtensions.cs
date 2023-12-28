using Net.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace Net.Json
{
    public static class SerializationExtensions
    {
        public static JsonSerializerSettings CreateDefaultSettings()
        {
           var result= new JsonSerializerSettings()
            {
                Formatting = Formatting.None,
                Converters = new List<JsonConverter>()
                    {
                        DateTimeJsonConverter.Default,
                        BoolJsonConverter.Default,
                        ConcreteConverter.Default,
                        EnumJsonConverter.Default,
                        ExpressionJsonConverter.Default,
                        NumberJsonConverter.Default,
                        StringJsonConverter.Default,
                        TimeSpanJsonConverter.Default

                    },
                ContractResolver = new DynamicContractResolver(),
               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
           };
            return result;
        }
        static JsonSerializerSettings _DefaultSettings = CreateDefaultSettings();
        static JsonSerializerSettings _IndentedSettings = CreateIndentedSettings();
        public static JsonSerializerSettings DefaultSettings
        {
            get { return _DefaultSettings; }
            set
            {
                if (value.IsNull()) throw new Exception("Json serializer settings must not be null");
                _DefaultSettings = value;
                _IndentedSettings = CreateIndentedSettings();
            }
        }
        static JsonSerializerSettings IndentedSettings { get; set; }= CreateIndentedSettings();

        private static JsonSerializerSettings CreateIndentedSettings()
        {
            var result = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                Converters = _DefaultSettings.Converters,
                ContractResolver = _DefaultSettings.ContractResolver,
            };
            return result;
        }

        public static string Serialize(this object item,bool indent=false)
        {
            if (item == null) return string.Empty;
           var result= JsonConvert.SerializeObject(item,indent?
               IndentedSettings: DefaultSettings);
            return result;

        }
        public static T Deserialize<T>(this string serializationText)
            => (T)Deserialize(serializationText, typeof(T));
        public static object Deserialize(this string serializationText,Type deserializeType)
        {
            if (serializationText.IsEmpty()) return null;
            return JsonConvert.DeserializeObject(serializationText,deserializeType, DefaultSettings);
        }
        public static ExpandoObject AsExpandoObject(this object obj)
        {
            if (obj.IsNull()) return new ExpandoObject();
            if (obj is ExpandoObject expobj) return expobj;
            if (obj is IEnumerable) return new ExpandoObject();
            var type = obj.GetType();
            if (type.IsValueType) return new ExpandoObject();
            return obj.Serialize().Deserialize<ExpandoObject>();
        }
    }
}
