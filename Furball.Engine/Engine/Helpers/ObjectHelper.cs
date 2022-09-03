using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Furball.Engine.Engine.Helpers;

public static class ObjectHelper {
    public static IEnumerable<pT> GetEnumerableOfType <pT>(params object[] constructorArgs) where pT : class, IComparable<pT> {
        List<pT> objects = Assembly.GetAssembly(typeof(pT)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(pT)))
                                  .Select(type => (pT)Activator.CreateInstance(type, constructorArgs)).ToList();
        objects.Sort();
        return objects;
    }

    public static List<FieldInfo> GetAllFieldsWithAttribute(Type classToCheckType, Type attributeType)
        => classToCheckType.GetFields().Where(p => p.GetCustomAttributes(attributeType, true).Any()).ToList();
}