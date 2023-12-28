using Newtonsoft.Json;
using System;

namespace Net.Json
{
    public class EnumJsonConverter : JsonConverter
    {
        public static readonly EnumJsonConverter Default = new EnumJsonConverter();

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsEnum;
        }

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                var value = reader.Value == null ? Activator.CreateInstance(objectType) : Enum.Parse(objectType, Convert.ToString(reader.Value));

                return value ?? Activator.CreateInstance(objectType);
            }
            catch
            {
                return Activator.CreateInstance(objectType);
            }

            //if (reader.TokenType == JsonToken.String)
            //{
            //    if (reader.Value == null)
            //    {
            //        return Activator.CreateInstance(objectType);
            //    }
            //    return Enum.Parse(objectType, reader.Value as string);
            //}
            //return existingValue == null ? Activator.CreateInstance(objectType) : existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");
        }
    }
}
