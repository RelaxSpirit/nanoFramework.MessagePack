// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

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
    }
}
