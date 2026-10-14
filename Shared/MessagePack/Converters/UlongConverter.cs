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
    internal class UlongConverter : IConverter
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
            if (value is ulong ulongValue)
            {
                NumberConverterHelper.WriteNonNegativeInteger(ulongValue, writer);
            }
            else
            {
#if NANOFRAMEWORK_1_0
                throw new ArgumentException();
#else
                throw new ArgumentException("Value must be of type ulong.", nameof(value));
#endif
            }
        }

        object? IConverter.Read([NotNull] IMessagePackReader reader)
        {
            DataTypes type = reader.ReadDataType();

            if (NumberConverterHelper.TryGetFixPositiveNumber(type, out byte temp))
            {
                return (ulong)temp;
            }

            if (NumberConverterHelper.TryGetNegativeNumber(type, out sbyte tempInt8))
            {
                return (ulong)tempInt8;
            }

            return type switch
            {
                DataTypes.UInt8 => (ulong)reader.ReadByte(),
                DataTypes.UInt16 => (ulong)NumberConverterHelper.ReadUInt16(reader),
                DataTypes.UInt32 => (ulong)NumberConverterHelper.ReadUInt32(reader),
                DataTypes.UInt64 => NumberConverterHelper.ReadUInt64(reader),
                DataTypes.Int8 => (ulong)NumberConverterHelper.ReadInt8(reader),
                DataTypes.Int16 => (ulong)NumberConverterHelper.ReadInt16(reader),
                DataTypes.Int32 => (ulong)NumberConverterHelper.ReadInt32(reader),
                DataTypes.Int64 => (ulong)reader.ReadInt64(),
                _ => throw ExceptionUtility.BadTypeException(type, DataTypes.PositiveFixNum, DataTypes.NegativeFixNum, DataTypes.UInt8, DataTypes.UInt16, DataTypes.UInt32, DataTypes.Int8, DataTypes.Int16, DataTypes.Int32, DataTypes.Int64, DataTypes.UInt64)
            };
        }
    }
}
