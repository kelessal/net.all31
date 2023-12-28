using Newtonsoft.Json;
using System;

namespace Net.Json
{
    public class StringJsonConverter : JsonConverter
    {
        public static readonly StringJsonConverter Default = new StringJsonConverter();

        static readonly Type StringType = typeof(string);

        public override bool CanConvert(Type objectType) => objectType == StringType;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return Convert.ToString(reader.Value)?.TrimEnd();
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");
        }
    }
}
