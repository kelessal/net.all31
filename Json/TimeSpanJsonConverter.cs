using Newtonsoft.Json;
using System;

namespace Net.Json
{
    public class TimeSpanJsonConverter : JsonConverter
    {
        public static readonly TimeSpanJsonConverter Default = new TimeSpanJsonConverter();

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan) || objectType==typeof(TimeSpan?);
        }

        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                if (objectType == typeof(TimeSpan)) return TimeSpan.MinValue;
                return null;
            }
            if (reader.ValueType == typeof(string))
            {

                if (TimeSpan.TryParse(reader.Value as string, out TimeSpan result)) return result;
                if( objectType==typeof(TimeSpan)) return TimeSpan.MinValue;
                return null;
            }
            var val = Convert.ToInt64(reader.Value);
            var netticks = 10000 * val ;
            netticks = System.Math.Min(TimeSpan.MaxValue.Ticks, netticks);
            netticks = System.Math.Max(TimeSpan.MinValue.Ticks, netticks);
            return new TimeSpan(netticks);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is TimeSpan t)
            {
                var jsonticks = t.Ticks/10000;
                writer.WriteValue(jsonticks);
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}
