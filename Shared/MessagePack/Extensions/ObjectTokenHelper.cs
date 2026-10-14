// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Dto;
using nanoFramework.MessagePack.Stream;
using nanoFramework.MessagePack.Utility;

namespace nanoFramework.MessagePack.Extensions
{
    internal static class ObjectTokenHelper
    {
#nullable enable
        internal static Hashtable? GetMessagePackObjectTokens([NotNull] this IMessagePackReader reader)
        {
            int length = (int)reader.ReadMapLength();
            return length > 0 ? reader.ReadMap(length) : null;
        }

        private static Hashtable ReadMap(this IMessagePackReader reader, int length)
        {
            var map = new Hashtable(length);

            while (length-- > 0)
            {
                if (ConverterContext.StringConverter.Read(reader) is string key && key != null)
                {
                    ArraySegment? value = reader.ReadToken();

                    map[key] = value;
                }
                else
                {
                    throw ExceptionUtility.BadTypeException(DataTypes.Null, DataTypes.FixStr);
                }
            }

            return map;
        }
    }
}
