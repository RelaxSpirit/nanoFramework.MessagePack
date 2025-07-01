// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
#if !NANOFRAMEWORK_1_0
using System.Linq;
#endif
using System.Reflection;
using nanoFramework.MessagePack.Dto;

namespace nanoFramework.MessagePack.Utility
{
    internal static class PropertyUtility
    {
        internal static void Map(Type targetType, ArrayList mappings)
        {
            var properties = GetProperties(targetType);

            if (properties == null || properties.Length == 0)
            {
                return;
            }

            foreach (var property in properties)
            {
                mappings.Add(new MemberMapping(property));
            }
        }
#if NANOFRAMEWORK_1_0
        private static MethodInfo[] GetProperties(Type type)
        {
            var list = new ArrayList();

            GetProperties(type, list);

            return (MethodInfo[])list.ToArray(typeof(MethodInfo));
        }

        private static void GetProperties(Type type, ArrayList list)
        {
            if (type == null)
            {
                return;
            }

            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (MethodInfo method in methods)
            {
                if (ShouldSerializeMethod(method))
                {
                    list.Add(method);
                }
            }

            //GetProperties(type.BaseType!, list, methodPrefix);
        }

        private static bool ShouldSerializeMethod(MethodInfo method)
        {
            // We care only about property getters when serializing and setter deserializing
            if (!method.Name.StartsWith(MemberMapping.GET_) && !method.Name.StartsWith(MemberMapping.SET_))
            {
                return false;
            }

            // Ignore abstract and virtual objects
            if (method.IsAbstract)
            {
                return false;
            }

            // Ignore static methods
            if (method.IsStatic)
            {
                return false;
            }

            // Ignore delegates and MethodInfos
            if ((method.ReturnType == typeof(Delegate)) ||
                (method.ReturnType == typeof(MulticastDelegate)) ||
                (method.ReturnType == typeof(MethodInfo)))
            {
                return false;
            }

            // Ditto for DeclaringType
            if ((method.DeclaringType == typeof(Delegate)) ||
                (method.DeclaringType == typeof(MulticastDelegate)))
            {
                return false;
            }

            return true;
        }
#else
        private static PropertyInfo[] GetProperties(Type type)
        {
            var list = new ArrayList();

            GetProperties(type, list);

            return (PropertyInfo[])list.ToArray(typeof(PropertyInfo));
        }

        private static void GetProperties(Type type, ArrayList list)
        {
            if (type == null)
            {
                return;
            }

            var propertyInfos = list.Cast<PropertyInfo>();

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(c => !propertyInfos.Any(x => x.Name == c.Name)).ToArray();
            
            foreach (PropertyInfo property in properties)
            {
                if (property.DeclaringType == type) //skip fields not declared on type, we will find it in a later recursion
                {
                    list.Add(property);
                }
            }

            GetProperties(type.BaseType!, list);
        }
#endif
    }
}
