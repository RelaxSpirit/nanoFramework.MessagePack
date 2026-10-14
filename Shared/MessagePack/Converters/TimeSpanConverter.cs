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
    internal class TimeSpanConverter : IConverter
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
            
            if (value is TimeSpan timeSpanValue)
            {
                ConverterContext.LongConverter.Write(timeSpanValue.Ticks, writer);
            }
            else
            {
#if NANOFRAMEWORK_1_0
                throw new ArgumentException();
#else
                throw new ArgumentException("Value must be of type TimeSpan.", nameof(value));
#endif
            }
        }

        object? IConverter.Read([NotNull] IMessagePackReader reader)
        {
            if (ConverterContext.LongConverter.Read(reader) is long longValue)
            {
                return TimeSpan.FromTicks(longValue);
            }
            
            throw ExceptionUtility.BadTypeException(DataTypes.Null, DataTypes.Int64);
        }
    }
}
