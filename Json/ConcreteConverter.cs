using Net.Proxy;
using Newtonsoft.Json;
using System;
namespace Net.Json
{
    public class ConcreteConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsInterface && !objectType.IsCollectionType();
        public static readonly ConcreteConverter Default = new ConcreteConverter();
        public override object ReadJson(JsonReader reader,
         Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, InterfaceType.GetProxyType(objectType));
        }

        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
