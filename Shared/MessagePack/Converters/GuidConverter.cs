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
    internal class GuidConverter : IConverter
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
            if (value is Guid guidValue)
            {
                ConverterContext.BinaryConverter.Write(guidValue.ToByteArray(), writer);
            }
            else
            {
#if NANOFRAMEWORK_1_0
                throw new ArgumentException();
#else
                throw new ArgumentException("Value must be of type Guid.", nameof(value));
#endif
            }
        }

        object? IConverter.Read([NotNull] IMessagePackReader reader)
        {
            if (ConverterContext.BinaryConverter.Read(reader) is byte[] byteArray)
            {
                return new Guid(byteArray);
            }
            
            throw ExceptionUtility.BadTypeException(DataTypes.Null, DataTypes.Bin8);
        }
    }
}
