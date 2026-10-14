// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Collections;
using System.Reflection;
using nanoFramework.MessagePack.Dto;

namespace nanoFramework.MessagePack.Utility
{
    internal static class FieldUtility
    {
        public static void Map(Type targetType, ArrayList mappings)
        {
            FieldInfo[] fields = GetFields(targetType);

            foreach (FieldInfo field in fields)
            {
                mappings.Add(new MemberMapping(field));
            }
        }

        private static FieldInfo[] GetFields(Type type)
        {
            var list = new ArrayList();

            GetFields(type, list);

            return (FieldInfo[])list.ToArray(typeof(FieldInfo));
        }

        private static void GetFields(Type type, ArrayList list)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                if (field.DeclaringType == type)
                {
                    list.Add(field);
                }
            }

            if (type.BaseType != null)
            {
                GetFields(type.BaseType, list);
            }
        }
    }
}
