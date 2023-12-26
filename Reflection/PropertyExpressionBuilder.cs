using Net.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Net.Reflection
{
    static class PropertyExpressionBuilder
    {
        internal static Func<object, object> CreateGetterFunc(Type type, string propName)
        {
            var parameterExpression = Expression.Parameter(typeof(object), "x");
            Expression curExpression = Expression.Convert(parameterExpression, type);
            PropertyInfo curInfo = null;
            foreach (var name in propName.SplitBy("."))
            {
                curInfo = curInfo == null ? type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance) :
                    curInfo.PropertyType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (curInfo == null) return null;
                curExpression = Expression.Property(curExpression, curInfo);
            }
            curExpression = Expression.Convert(curExpression, typeof(object));
            return Expression.Lambda<Func<object, object>>(curExpression, parameterExpression).Compile();
        }
        internal static Action<object, object> CreateSetterFunc(this Type type, string propName)
        {
            var parameterExpression = Expression.Parameter(typeof(object), "x");
            var valueExpression = Expression.Parameter(typeof(object), "y");
            Expression curExpression = Expression.Convert(parameterExpression, type);
            PropertyInfo curInfo = null;
            foreach (var name in propName.SplitBy("."))
            {
                curInfo = curInfo == null ? type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance) :
                    curInfo.PropertyType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (curInfo == null) return null;
                curExpression = Expression.Property(curExpression, curInfo);
            }
            curExpression = Expression.Assign(curExpression, Expression.Convert(valueExpression, curInfo.PropertyType));
            return Expression.Lambda<Action<object, object>>(curExpression, parameterExpression, valueExpression).Compile();
        }
    }
}
