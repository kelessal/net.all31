using Serialize.Linq.Extensions;
using Serialize.Linq.Serializers;
using System;
using System.Linq.Expressions;

namespace Net.Json
{
    static class ExpressionExtensions
    {
        public static string ToJsonText(this Expression expression)
        {
            return expression.ToJson();
        }
        public static Expression JsonTextToExpression(this string text)
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());
            return serializer.DeserializeText(text);
        }
        public static LambdaExpression ExtendLambdaExpression(this LambdaExpression expression,Func<Expression,Expression> extendFn)
        {
            var result = new LambdaTopMemberExtendVisitor(extendFn).Visit(expression.Body) as LambdaExpression;
            return Expression.Lambda(result.Body, expression.Parameters);
        }
    }
}
