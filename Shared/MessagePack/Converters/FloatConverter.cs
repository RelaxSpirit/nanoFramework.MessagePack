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
    internal class FloatConverter : IConverter
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

            if (value is float floatValue)
            {
                byte[] binary = BitConverter.GetBytes(floatValue);
                byte[] bytes;
                if (BitConverter.IsLittleEndian)
                {
                    bytes = new[]
                    {
                        (byte)DataTypes.Single,
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
                        (byte)DataTypes.Single,
                        binary[0],
                        binary[1],
                        binary[2],
                        binary[3]
                    };
                }

                writer.Write(bytes);
            }
            else
            {
#if NANOFRAMEWORK_1_0
                throw new ArgumentException();
#else
                throw new ArgumentException("Value must be of type System.Single (float).", nameof(value));
#endif
            }
        }

        object? IConverter.Read([NotNull] IMessagePackReader reader)
        {
            DataTypes type = reader.ReadDataType();

            if (type == DataTypes.Single)
            {
                return NumberConverterHelper.ReadFloat(reader);
            }

            if (NumberConverterHelper.TryGetInt32(type, reader, out int result))
            {
                return (float)result;
            }
            else
            {
                throw ExceptionUtility.BadTypeException(type, DataTypes.Single);
            }
        }
    }
}
