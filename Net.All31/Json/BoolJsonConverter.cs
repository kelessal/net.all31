using Newtonsoft.Json;
using System;

namespace Net.Json
{
    public class BoolJsonConverter : JsonConverter
    {
        public static readonly BoolJsonConverter Default = new BoolJsonConverter();


        public override bool CanConvert(Type objectType) => objectType == typeof(bool);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                var value = Convert.ToBoolean(reader.Value);
                return value;
            }
            catch
            {
                return false;
            }
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");
        }
    }
}
