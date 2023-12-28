using Newtonsoft.Json;
using System;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Net.Json
{
    public class ExpressionJsonConverter : JsonConverter
    {
        private static readonly Type TypeOfExpression = typeof(Expression);

        public static readonly ExpressionJsonConverter Default = new ExpressionJsonConverter();

        public override bool CanConvert(Type objectType)
        {
            return objectType == TypeOfExpression
                || objectType.IsSubclassOf(TypeOfExpression);
        }

        public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer)
        {
            var expression = value as Expression;
            if (expression != null)
            {
                var str= expression.ToJsonText();
                var result = JToken.FromObject(str);
                result.WriteTo(writer);
                return;
            }
            writer.WriteNull();
        }

        public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return (reader.Value as string).JsonTextToExpression();
        }

    }
}
