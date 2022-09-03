using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Furball.Engine.Engine.Helpers;

public static class ObjectHelper {
    public static IEnumerable<T> GetEnumerableOfType <T>(params object[] constructorArgs) where T : class, IComparable<T> {
        List<T> objects = Assembly.GetAssembly(typeof(T)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)))
                                  .Select(type => (T)Activator.CreateInstance(type, constructorArgs)).ToList();
        objects.Sort();
        return objects;
    }

    public static List<FieldInfo> GetAllFieldsWithAttribute(Type classToCheckType, Type attributeType)
        => classToCheckType.GetFields().Where(p => p.GetCustomAttributes(attributeType, true).Any()).ToList();
}