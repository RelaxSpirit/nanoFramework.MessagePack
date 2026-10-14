// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif
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
#if NANOFRAMEWORK_1_0
            PropertyInfo[] properties = MemberPropertyInfo.GetProperties(targetType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#else
            PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif

            foreach (PropertyInfo property in properties)
            {
                mappings.Add(new MemberMapping(property));
            }
        }
    }
}
