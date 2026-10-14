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
    internal class ShortConverter : IConverter
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
            
            if (value is short shortValue)
            {
                NumberConverterHelper.WriteInteger(shortValue, writer);
            }
            else
            {
#if NANOFRAMEWORK_1_0
                throw new ArgumentException();
#else
                throw new ArgumentException("Value must be of type short.", nameof(value));
#endif
            }
        }

        object IConverter.Read([NotNull] IMessagePackReader reader)
        {
            DataTypes type = reader.ReadDataType();

            if (NumberConverterHelper.TryGetFixPositiveNumber(type, out byte temp))
            {
                return (short)temp;
            }

            if (NumberConverterHelper.TryGetNegativeNumber(type, out sbyte tempInt8))
            {
                return (short)tempInt8;
            }

            switch (type)
            {
                case DataTypes.UInt8:
                    return reader.ReadByte();

                case DataTypes.UInt16:
                    ushort ushortValue = NumberConverterHelper.ReadUInt16(reader);
                    if (ushortValue <= short.MaxValue)
                    {
                        return (short)ushortValue;
                    }

                    throw ExceptionUtility.BadTypeException(type, DataTypes.UInt16);

                case DataTypes.Int8:
                    return (short)NumberConverterHelper.ReadInt8(reader);

                case DataTypes.Int16:
                    return NumberConverterHelper.ReadInt16(reader);
                default:
                    throw ExceptionUtility.BadTypeException(type, DataTypes.PositiveFixNum, DataTypes.NegativeFixNum, DataTypes.UInt8, DataTypes.Int8, DataTypes.Int16);
            }
        }
    }
}
