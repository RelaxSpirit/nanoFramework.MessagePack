// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
#if !NANOFRAMEWORK_1_0
using System.Collections.Generic;
#endif

namespace MessagePack.Extensions
{
    internal static class TypeHelper
    {
        internal static bool IsImplementInterface(this Type sourceType, Type targetInterfaceType)
        {
            foreach(Type interfaceType in  sourceType.GetInterfaces())
            {
                if (interfaceType.Name == targetInterfaceType.Name)
                    return true;
            }

            return false;
        }

        internal static bool IsGenericDictionary(this Type sourceType)
        {
#if NANOFRAMEWORK_1_0
            return false;
#else
            return sourceType.IsInterface && sourceType.IsGenericType && typeof(IDictionary<,>).Name == sourceType.Name;
#endif
            
        }
    }
}
