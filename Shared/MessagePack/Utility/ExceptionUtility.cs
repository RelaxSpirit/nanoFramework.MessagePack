// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif
using nanoFramework.MessagePack.Exceptions;
using nanoFramework.MessagePack.Extensions;

namespace nanoFramework.MessagePack.Utility
{
    internal static class ExceptionUtility
    {
        public static Exception BadTypeException(DataTypes actual, params DataTypes[] expectedCodes)
        {
            return new SerializationException($"Got {actual:G} (0x{actual:X}), while expecting one of these: {expectedCodes.JoinToString(", ")}");
        }

        public static Exception NotEnoughBytes(int actual, int expected)
        {
            return NotEnoughBytes((long)actual, (long)expected);
        }

        public static Exception NotEnoughBytes(long actual, long expected)
        {
            return new SerializationException($"Expected {expected} bytes, got {actual} bytes.");
        }

        public static Exception NoConverterForCollectionElement(Type type, string elementName)
        {
            return new SerializationException($"No converter provided for element '{elementName}' of type {type.Name}.");
        }

        public static Exception NumberSerializationFailure(long value)
        {
            return NumberSerializationFailure((ulong)value);
        }

        public static Exception NumberSerializationFailure(ulong value)
        {
            return new SerializationException($"Can't serialize {value}");
        }

        public static Exception ConverterNotFound(Type type)
        {
            return new ConverterNotFoundException(type);
        }
    }
}
