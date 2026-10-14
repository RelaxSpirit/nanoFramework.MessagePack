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
    internal class LongConverter : IConverter
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

            if (value is long longValue)
            {
                NumberConverterHelper.WriteInteger(longValue, writer);
            }
            else
            {
#if NANOFRAMEWORK_1_0
                throw new ArgumentException();
#else
                throw new ArgumentException("Value must be of type long.", nameof(value));
#endif
            }
        }

        object? IConverter.Read([NotNull] IMessagePackReader reader)
        {
            DataTypes type = reader.ReadDataType();

            if (NumberConverterHelper.TryGetInt64(type, reader, out long result))
            {
                return result;
            }
            else
            {
                throw ExceptionUtility.BadTypeException(type, DataTypes.PositiveFixNum, DataTypes.NegativeFixNum, DataTypes.UInt8, DataTypes.UInt16, DataTypes.UInt32, DataTypes.Int8, DataTypes.Int16, DataTypes.Int32, DataTypes.Int64);
            }
        }
    }
}
