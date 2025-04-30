// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;

namespace nanoFramework.MessagePack.Extensions
{
    internal static class DataTypesExtension
    {
        internal static byte GetHighBits(this DataTypes type, byte bitsCount)
        {
            return (byte)((byte)type >> (8 - bitsCount));
        }

        internal static string JoinToString(this DataTypes[] types, string separator)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in types)
            {
                sb.Append(item);
                sb.Append(separator);
            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - separator.Length, separator.Length);
            }

            return sb.ToString();
        }
    }
}
