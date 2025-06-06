// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Stream;

namespace nanoFramework.MessagePack.Extensions
{
    internal static class ObjectTokenHelper
    {
#nullable enable
        internal static Hashtable GetMassagePackObjectTokens([NotNull] this IMessagePackReader reader)
        {
            var length = reader.ReadMapLength();
            return reader.ReadMap(length);
        }

        private static Hashtable ReadMap(this IMessagePackReader reader, uint length)
        {
            var map = new Hashtable();

            var stringConverter = ConverterContext.GetConverter(typeof(string));
            for (var i = 0; i < length; i++)
            {
                var key = stringConverter.Read(reader);
                var value = reader.ReadToken();

                map[key!] = value;
            }

            return map;
        }
    }
}
