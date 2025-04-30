// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Extensions;
using nanoFramework.MessagePack.Stream;
using nanoFramework.MessagePack.Utility;

namespace nanoFramework.MessagePack.Converters
{
    internal class DoubleConverter : IConverter
    {
        private static void Write(double value, IMessagePackWriter writer)
        {
            var binary = BitConverter.GetBytes(value);
            byte[] bytes;
            if (BitConverter.IsLittleEndian)
            {
                bytes = new[]
                {
                    (byte) DataTypes.Double,
                    binary[7],
                    binary[6],
                    binary[5],
                    binary[4],
                    binary[3],
                    binary[2],
                    binary[1],
                    binary[0]
                };
            }
            else
            {
                bytes = new[]
                {
                    (byte) DataTypes.Double,
                    binary[0],
                    binary[1],
                    binary[2],
                    binary[3],
                    binary[4],
                    binary[5],
                    binary[6],
                    binary[7]
                };
            }

            writer.Write(bytes);
        }

        private static double Read(IMessagePackReader reader)
        {
            var type = reader.ReadDataType();

            if (type != DataTypes.Single && type != DataTypes.Double)
            {
                //throw ExceptionUtility.BadTypeException(type, DataTypes.Single, DataTypes.Double);

                if (NumberConverterHelper.TryGetInt64(type, reader, out var result))
                {
                    return result;
                }
                else
                {
                    throw ExceptionUtility.BadTypeException(type, DataTypes.Single, DataTypes.Double);
                }
            }

            if (type == DataTypes.Single)
            {
                return NumberConverterHelper.ReadFloat(reader);
            }

            return NumberConverterHelper.ReadDouble(reader);
        }

#nullable enable
        public void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            Write((double)value!, writer);
        }

        object? IConverter.Read([NotNull] IMessagePackReader reader)
        {
            return Read(reader);
        }
    }
}
