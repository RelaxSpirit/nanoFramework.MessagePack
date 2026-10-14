// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Extensions;
using nanoFramework.MessagePack.Stream;
using nanoFramework.MessagePack.Utility;

namespace nanoFramework.MessagePack.Converters
{
    internal class ByteConverter : IConverter
    {
#nullable enable
        public void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            if (value == null)
            {
#if NANOFRAMEWORK_1_0
                throw new ArgumentNullException();
#else
                throw new ArgumentNullException(nameof(value));
#endif
            }

            if (value is byte byteValue)
            {
                // positive fixnum
                if (byteValue < 128L)
                {
                    writer.Write(byteValue);
                }
                else
                {
                    writer.Write(DataTypes.UInt8);
                    writer.Write(byteValue);
                }
            }
            else
            {
#if NANOFRAMEWORK_1_0
                throw new ArgumentException();
#else
                throw new ArgumentException("Value must be of type byte.", nameof(value));
#endif
            }
        }

        object? IConverter.Read([NotNull] IMessagePackReader reader)
        {
            DataTypes type = reader.ReadDataType();

            if (NumberConverterHelper.TryGetFixPositiveNumber(type, out byte temp))
            {
                return temp;
            }

            if (NumberConverterHelper.TryGetNegativeNumber(type, out sbyte tempInt8))
            {
                return (byte)tempInt8;
            }

            return type switch
            {
                DataTypes.UInt8 => reader.ReadByte(),
                DataTypes.Int8 => (byte)NumberConverterHelper.ReadInt8(reader),
                _ => throw ExceptionUtility.BadTypeException(type, DataTypes.PositiveFixNum, DataTypes.NegativeFixNum, DataTypes.UInt8, DataTypes.Int8),
            };
        }
    }
}
