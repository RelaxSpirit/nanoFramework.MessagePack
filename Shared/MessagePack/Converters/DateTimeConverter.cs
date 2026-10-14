// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Stream;
using nanoFramework.MessagePack.Utility;

namespace nanoFramework.MessagePack.Converters
{
    internal class DateTimeConverter : IConverter
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

            if (value is DateTime dateTimeValue)
            {
                var longValue = dateTimeValue.Subtract(DateTime.UnixEpoch).Ticks;

                ConverterContext.LongConverter.Write(longValue, writer);
            }
            else
            {
#if NANOFRAMEWORK_1_0
                throw new ArgumentException();
#else
                throw new ArgumentException("Value must be a non-null DateTime.", nameof(value));
#endif
            }
        }

        object? IConverter.Read([NotNull] IMessagePackReader reader)
        {
            if (ConverterContext.LongConverter.Read(reader) is long value)
            {
                return DateTime.UnixEpoch.AddTicks(value);
            }

            throw ExceptionUtility.BadTypeException(DataTypes.Null, DataTypes.Int64);
        }
    }
}
