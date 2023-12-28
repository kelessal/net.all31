using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Net.Json
{
    static class ReflectionExtensions
    {
        public static bool IsCollectionType(this Type type)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(type)) return false;
            if (type.IsArray && type.GetArrayRank() == 1) return true;
            if (!type.IsGenericType) return false;
            if (type.GetGenericArguments().Length != 1) return false;
            return typeof(IEnumerable<>).MakeGenericType(GetCollectionElementType(type)).IsAssignableFrom(type);
        }
        public static Type GetCollectionElementType(this Type type)
        {
            if (type.IsArray) return type.GetElementType();
            if (!type.IsGenericType) return null;
            if (type.GetGenericArguments().Length > 1) return null;
            return type.GetGenericArguments()[0];
        }
        public static bool IsAssignableTo<T>(this Type type)
           => typeof(T).IsAssignableFrom(type);

    }
}
