// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Stream;
using nanoFramework.MessagePack.Utility;

namespace nanoFramework.MessagePack.Converters
{
    /// <summary>
    /// Simple array converter
    /// </summary>
    public class SimpleArrayConverter : IConverter
    {
        private readonly Type _elementType;

        /// <summary>
        /// Simple array converter
        /// </summary>
        /// <param name="elementType">Type of array element</param>
        public SimpleArrayConverter(Type elementType)
        {
            _elementType = elementType;
        }

        /// <summary>
        /// Write array in to MessagePack stream
        /// </summary>
        /// <param name="value">Array to by write</param>
        /// <param name="writer">MessagePack stream for written</param>
        public void Write(Array value, IMessagePackWriter writer)
        {
            if (value == null)
            {
                ConverterContext.NullConverter.Write(value, writer);
                return;
            }

            writer.WriteArrayHeader((uint)value.Length);

            if (value.Length > 1)
            {
                var converter = ConverterContext.GetConverter(_elementType) ?? throw ExceptionUtility.NoConverterForCollectionElement(_elementType, "item");

                foreach (var element in value)
                {
                    converter.Write(element, writer);
                }
            }
        }

        /// <summary>
        /// Read array from MessagePack stream
        /// </summary>
        /// <param name="reader">MessagePack stream for reading</param>
        /// <returns>Readied array</returns>
#nullable enable
        public Array? Read(IMessagePackReader reader)
        {
            var length = (int)reader.ReadArrayLength();

            return length > -1 ? ReadArray(reader, length) : null;
        }

        private Array ReadArray(IMessagePackReader reader, int length)
        {
            var converter = ConverterContext.GetConverter(_elementType);

            var result = (IList)Array.CreateInstance(_elementType, length);

            if (converter != null)
            {
                for (var i = 0; i < length; i++)
                {
                    result[i] = converter.Read(reader)!;
                }
            }
            else
            {
                for (var i = 0; i < length; i++)
                {
                    result[i] = ConverterContext.DeserializeObject(_elementType, reader);
                }
            }

            return (Array)result;
        }

        void IConverter.Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            Write((Array)value!, writer);
        }

        object? IConverter.Read([NotNull] IMessagePackReader reader)
        {
            return Read(reader)!;
        }
    }
}
