using Newtonsoft.Json;
using System;

namespace Net.Json
{
    public class NumberJsonConverter : JsonConverter
    {
        public static readonly NumberJsonConverter Default = new NumberJsonConverter();

        public override bool CanConvert(Type objectType)
        {
            return IsNumericType(objectType);
        }

        private static bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                var value = reader.Value == null ? Activator.CreateInstance(objectType) : Convert.ChangeType(reader.Value, objectType);

                return value ?? Activator.CreateInstance(objectType);
            }
            catch (Exception)
            {
                return Activator.CreateInstance(objectType);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");
        }
    }
}
