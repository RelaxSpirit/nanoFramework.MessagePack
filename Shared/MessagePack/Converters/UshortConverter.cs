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
    internal class UshortConverter : IConverter
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
            if (value is ushort ushortValue)
            {
                NumberConverterHelper.WriteNonNegativeInteger(ushortValue, writer);
            }
            else
            {
#if NANOFRAMEWORK_1_0
                throw new ArgumentException();
#else
                throw new ArgumentException($"The type of {nameof(value)} is not ushort.");
#endif
            }
        }

        object? IConverter.Read([NotNull] IMessagePackReader reader)
        {
            DataTypes type = reader.ReadDataType();

            if (NumberConverterHelper.TryGetFixPositiveNumber(type, out byte temp))
            {
                return (ushort)temp;
            }

            if (NumberConverterHelper.TryGetNegativeNumber(type, out sbyte tempInt8))
            {
                return (ushort)tempInt8;
            }

            return type switch
            {
                DataTypes.UInt8 => (ushort)reader.ReadByte(),
                DataTypes.UInt16 => NumberConverterHelper.ReadUInt16(reader),
                DataTypes.Int8 => (ushort)NumberConverterHelper.ReadInt8(reader),
                DataTypes.Int16 => (ushort)NumberConverterHelper.ReadInt16(reader),
                _ => throw ExceptionUtility.BadTypeException(type, DataTypes.PositiveFixNum, DataTypes.NegativeFixNum, DataTypes.UInt8, DataTypes.Int8, DataTypes.Int16, DataTypes.UInt16)
            };
        }
    }
}
