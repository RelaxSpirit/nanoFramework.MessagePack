// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !NANOFRAMEWORK_1_0
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using nanoFramework.MessagePack.Dto;
using nanoFramework.MessagePack.Stream;

namespace nanoFramework.MessagePack.Converters
{
    internal class NullableConverter<ValueType> : IConverter where ValueType : struct
    {
      #nullable enable
        internal static ValueType? Read([NotNull] IMessagePackReader reader)
        {
            ArraySegment arraySegment = reader.ReadToken()!;
            DataTypes type = arraySegment.ReadDataType();
            if (type == DataTypes.Null)
            {
                return default;
            }
            else
            {
                arraySegment.Seek(0, SeekOrigin.Begin);
                return (ValueType)ConverterContext.GetConverter(typeof(ValueType)).Read(arraySegment)!;
            }
        }

        object? IConverter.Read([NotNull] IMessagePackReader reader)
        {
            return Read(reader);
        }
        public void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            if (value != null)
                ConverterContext.GetConverter(typeof(ValueType)).Write((ValueType)value, writer);
        }
    }
}
#endif
