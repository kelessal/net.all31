using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Net.Reflection
{
    internal class MethodCollection
    {
        ConcurrentDictionary<string, MethodInfo> _methods = new ConcurrentDictionary<string, MethodInfo>();
        public Type Type { get; private set; }
        public MethodCollection(Type type)
        {
            this.Type = type;
        }

        public MethodInfo SearchMethod(string methodName, Func<MethodInfo, bool> finder, params Type[] genericParameters)
        {
            var key = methodName + "#" + (genericParameters.Length == 0 ? "" : genericParameters.Select(p => p.ToString()).Aggregate((pre, next) => $"{pre}#{next}"));

            if (_methods.ContainsKey(key)) return _methods[key];
            var found = this.Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .Where(p => p.Name == methodName).FirstOrDefault(finder);
            found = genericParameters.Length > 0 ? found.MakeGenericMethod(genericParameters) : found;
            _methods[methodName] = found;
            return found;
        }

    }
}
