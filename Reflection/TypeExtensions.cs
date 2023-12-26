using Net.Extensions;
using Net.Json;
using Net.Proxy;
using Net.Reflection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Net.Reflection
{
    public static class TypeExtensions
    {
        static readonly char[] PROP_SPLITTER = new char[] { '.' };

        static readonly ConcurrentDictionary<Type, MethodCollection> _genericMethods = new ConcurrentDictionary<Type, MethodCollection>();
        static readonly HashSet<Type> primitiveTypes = new HashSet<Type>(new[]
       {
            typeof(string),
            typeof(int),
            typeof(int?),
            typeof(double),
            typeof(double?),
            typeof(float),
            typeof(float?),
            typeof(decimal),
            typeof(decimal?),
            typeof(long),
            typeof(long?),
            typeof(Guid),
            typeof(Guid?),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(bool),
            typeof(bool?),
            //typeof(TimeSpan),
            //typeof(TimeSpan?),
            typeof(byte)
        });
        public static Type GetGenericClosedTypeOf(this Type type, Type genericTypeDef)
        {
            if (!genericTypeDef.IsGenericTypeDefinition)
                throw new SystemException("It is not generic type  definition");
            var baseType = type.BaseType;
            if (baseType == typeof(object) || baseType == null) return null;
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == genericTypeDef)
                return baseType;
            return baseType.GetGenericClosedTypeOf(genericTypeDef);
        }
        public static string GetFriendlyName(this Type type)
        {
            if (type == typeof(int))
                return "int";
            else if (type == typeof(short))
                return "short";
            else if (type == typeof(byte))
                return "byte";
            else if (type == typeof(bool))
                return "bool";
            else if (type == typeof(long))
                return "long";
            else if (type == typeof(float))
                return "float";
            else if (type == typeof(double))
                return "double";
            else if (type == typeof(decimal))
                return "decimal";
            else if (type == typeof(string))
                return "string";
            else if (type.IsGenericType)
                return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(x => GetFriendlyName(x)).ToArray()) + ">";
            else
                return type.Name;
        }
       
        public static PropertyInfo[] GetPropertyInfos(this Type type, string propName)
        {
            List<PropertyInfo> result = new List<PropertyInfo>();
            PropertyInfo curInfo = null;
            foreach (var name in propName.Split(PROP_SPLITTER, StringSplitOptions.RemoveEmptyEntries))
            {
                curInfo = curInfo == null ? type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance) :
                    curInfo.PropertyType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (curInfo == null) return null;
                result.Add(curInfo);
            }
            return result.Count == 0 ? null : result.ToArray();
        }
        public static PropertyInfo GetSubProperty(this Type type, string propName)
        {
            PropertyInfo curInfo = null;
            foreach (var name in propName.Split(PROP_SPLITTER, StringSplitOptions.RemoveEmptyEntries))
            {
                curInfo = curInfo == null ? type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance) :
                    curInfo.PropertyType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (curInfo == null) return null;
            }
            return curInfo;
        }
        
        public static bool IsNullableOf(this Type nullableType,Type type)
            => nullableType.IsGenericType && Nullable.GetUnderlyingType(nullableType) == type;
        
        public static Type GetCollectionElementType(this Type type)
        {
            if (type.IsArray) return type.GetElementType();
            if (!type.IsGenericType) return null;
            if (type.GetGenericArguments().Length > 1) return null;
            return type.GetGenericArguments()[0];
        }

         
        public static MethodInfo FindMethod(this Type type, string methodName, Func<MethodInfo, bool> finder, params Type[] genericParameters)
        {
            if (_genericMethods.ContainsKey(type))
                return _genericMethods[type].SearchMethod(methodName, finder, genericParameters);
            var gmcollection = new MethodCollection(type);
            _genericMethods[type] = gmcollection;
            return gmcollection.SearchMethod(methodName, finder, genericParameters);

        }
        public static PropertyInfo[] FindProperties(this Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(
                        BindingFlags.FlattenHierarchy
                        | BindingFlags.Public
                        | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy
                | BindingFlags.Public | BindingFlags.Instance);

        }
        public static TypeKind GetTypeKind(this Type type)
        {
            if (type.Name.Contains("AnonymousType")) return TypeKind.Complex;
            if (type.IsPrimitiveType()) return TypeKind.Primitive;
            if (type.IsCollectionType()) return TypeKind.Collection;
            if (!type.IsGenericTypeDefinition  &&(type.IsClass || type.IsInterface)) return TypeKind.Complex;
            return TypeKind.Unknown;
        }
      
        public static bool IsPrimitiveType(this Type type)
            => primitiveTypes.Contains(type) || type.IsEnum;
        public static bool IsAssignableTo<T>(this Type type)
            => typeof(T).IsAssignableFrom(type);

      
        public static bool HasInterface(this Type type, Type interfaceType)
        {
            if (!interfaceType.IsInterface)
                throw new SystemException("It is not interface");
            return type.GetInterfaces()
                .Any(p => {
                    if (p == interfaceType) return true;
                    if (!interfaceType.IsGenericTypeDefinition) return false;
                    if (!p.IsGenericType) return false;
                    return p.GetGenericTypeDefinition() == interfaceType;
                });
        }
        public static bool IsCollectionType(this Type type)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(type)) return false;
            if (type.IsArray && type.GetArrayRank() == 1) return true;
            if (!type.IsGenericType) return false;
            if (type.GetGenericArguments().Length != 1) return false;
            return typeof(IEnumerable<>).MakeGenericType(type.GetCollectionElementType()).IsAssignableFrom(type);
        }

        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            Type currentType = type.BaseType;
            while (currentType!=typeof(object))
            {
                yield return currentType;
                if (currentType == null) break;
                currentType = currentType.BaseType;
            }
        }
        public static TypeInfo GetInfo(this Type type)
        {
            return TypeInfo.GetTypeInfo(type);
        }


        public static void SetValue(this object item, string property, object value)
        {
            if (item is IDictionary<string, object> dicObject) // For Dynamic Objects
            {
                dicObject[property] = value;
            } else if(item is IDictionary<string,JToken> tokenDic)
            {
                tokenDic[property] =value is JToken tokenValue?tokenValue:JToken.FromObject(value);

            } else
            {
                var info = item.GetType().GetInfo()[property];
                if (info.IsNull()) return;
                info.SetValue(item, value);
            }
            
        }
        public static object GetPropValue(this object item,string property)
        {
            object value = null;
            if (item is IDictionary<string, object> dicObject) // For Dynamic Objects
            {
                if (!dicObject.ContainsKey(property)) return default;
                value = dicObject[property];
            }
            else if (item is IDictionary<string, JToken> tokenDic)
            {
                if (!tokenDic.ContainsKey(property)) return default;
                value = tokenDic[property];
            }
            else
            {
                var info = item.GetType().GetInfo();
                if (!info.HasProperty(property)) return default;
                value = info[property].GetValue<object>(item);
            }
            return value;
        }
        public static T GetPropValue<T>(this object item, string property)
        {
            var value = item.GetPropValue(property);
            if (value == null) return default;
            if (value is T tvalue) return tvalue;
            return value.Serialize().Deserialize<T>();
        }
        public static T GetPathValue<T>(this object item, string path)
        {
            var value = item.GetPathValue(path);
            if (value == null) return default;
            return value.As<T>();
        }
        public static object GetPathValue(this object item, string path)
        {
            if (item == null) return default;
            var left = path.TrimThenBy(".");
            var leftValue = item.GetPropValue<object>(left);
            if (leftValue == null) return default;
            if (left == path)
            {
                return leftValue;
            }
            var right = path.TrimLeftBy(".");
            return leftValue.GetPathValue(right);
        }
        public static void SetPathValue(this object item, string path,object value)
        {
            if (item == null) return;
            var left = path.TrimThenBy(".");
            if (left == path) {
                item.SetValue(left, value);
                return;
            }
            var leftValue = item.GetPropValue<object>(left);
            if (leftValue == null) return;
            var right = path.TrimLeftBy(".");
            leftValue.SetPathValue(right,value);
        }
        public static T As<T>(this object item)
        {
            if (item.IsNull()) return default;
            if (item is T titem) return titem;
            return item.Serialize().Deserialize<T>();
        }
        public static T AsCloned<T>(this object item)
        {
            if(item.IsNull()) return default;
            return item.Serialize().Deserialize<T>();
        }
        public static object AsCloned(this object item,Type type=default)
        {
            if (item.IsNull()) return default;
            if (type == null)
            {
                if (item is IProxyData)
                    type = InterfaceType.GetIntefaceTypeOfProxy(item);
                else
                    type=item.GetType();
            }

            return item.Serialize().Deserialize(type);
        }
        public static object As(this object item,Type asType)
        {
            if (item.IsNull()) return default;
            if (asType.IsAssignableFrom(item.GetType())) return item;
            return item.Serialize().Deserialize(asType);
        }
    }
}
