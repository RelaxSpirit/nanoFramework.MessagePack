// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Dto;
using nanoFramework.MessagePack.Extensions;
using nanoFramework.MessagePack.Stream;
using nanoFramework.MessagePack.Utility;

namespace nanoFramework.MessagePack.Converters
{
    internal class BinaryConverter : IConverter
    {
        private static void WriteBinaryHeaderAndLength(uint length, IMessagePackWriter writer)
        {
            if (length <= byte.MaxValue)
            {
                writer.Write(DataTypes.Bin8);
                writer.Write((byte)length);
            }
            else if (length <= ushort.MaxValue)
            {
                writer.Write(DataTypes.Bin16);
                NumberConverterHelper.WriteUShortValue((ushort)length, writer);
            }
            else
            {
                writer.Write(DataTypes.Bin32);
                NumberConverterHelper.WriteUIntValue(length, writer);
            }
        }

#nullable enable
        public void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            if (value == null)
            {
                ConverterContext.NullConverter.Write(value, writer);
                return;
            }

            if (value is byte[] byteArray)
            {
                uint valueLength = (uint)byteArray.Length;

                WriteBinaryHeaderAndLength(valueLength, writer);

                writer.Write(byteArray);
            }
            else
            {
#if NANOFRAMEWORK_1_0
                throw new ArgumentException();
#else
                throw new ArgumentException("The value must be of type byte[].", nameof(value));
#endif
            }
        }

        object? IConverter.Read([NotNull] IMessagePackReader reader)
        {
            DataTypes type = reader.ReadDataType();

            uint length;
            switch (type)
            {
                case DataTypes.Null:
                    return null;

                case DataTypes.Bin8:
                    length = reader.ReadByte();
                    break;

                case DataTypes.Bin16:
                    length = NumberConverterHelper.ReadUInt16(reader);
                    break;

                case DataTypes.Bin32:
                    length = NumberConverterHelper.ReadUInt32(reader);
                    break;

                default:
                    throw ExceptionUtility.BadTypeException(type, DataTypes.Bin8, DataTypes.Bin16, DataTypes.Bin32, DataTypes.Null);
            }

            ArraySegment array = reader.ReadBytes(length);
            return (byte[])array;
        }
    }
}
