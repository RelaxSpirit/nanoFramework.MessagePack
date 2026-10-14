// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Stream;
using nanoFramework.MessagePack.Utility;

namespace nanoFramework.MessagePack.Converters
{
    /// <summary>
    /// Simple array converter.
    /// </summary>
    public class SimpleArrayConverter : IConverter
    {
#nullable enable
        private readonly Type _elementType;

        /// <summary>
        /// Initializes a new instance of the SimpleArrayConverter class.
        /// </summary>
        /// <param name="elementType">Type of array element.</param>
        public SimpleArrayConverter(Type elementType)
        {
            _elementType = elementType;
        }

        /// <summary>
        /// Write array in to MessagePack stream.
        /// </summary>
        /// <param name="value">Array to by write.</param>
        /// <param name="writer">MessagePack stream for written.</param>
        public void Write(Array? value, IMessagePackWriter writer)
        {
            Write(value, _elementType, writer);
        }

        /// <summary>
        /// Read array from MessagePack stream.
        /// </summary>
        /// <param name="reader">MessagePack stream for reading.</param>
        /// <returns>Read array.</returns>
        public IList? Read(IMessagePackReader reader)
        {
            var length = (int)reader.ReadArrayLength();
            return length > -1 ? ReadArray(reader, length, _elementType ?? throw ExceptionUtility.ConverterNotFound(typeof(object))) : null;
        }

        internal static void Write(IList? value, Type? elementType, IMessagePackWriter writer)
        {
            if (value == null)
            {
                ConverterContext.NullConverter.Write(value, writer);
                return;
            }

            writer.WriteArrayHeader((uint)value.Count);
            if (value.Count > 0)
            {
                if (elementType == null)
                {
                    Type arrayType = value.GetType();
                    elementType = arrayType.GetElementType() ?? throw ExceptionUtility.ConverterNotFound(typeof(object));
                }

                var elementConverter = ConverterContext.GetConverter(elementType);
                if (elementConverter != null)
                {
                    foreach (var element in value)
                    {
                        elementConverter.Write(element, writer);
                    }
                }
                else
                {
                    foreach (var element in value)
                    {
                        ConverterContext.SerializeObject(elementType, element, writer);
                    }
                }
            }
        }

        internal static IList? Read(IMessagePackReader reader, Type arrayType)
        {
            int length = (int)reader.ReadArrayLength();
            Type? elementType = arrayType.GetElementType();
#if !NANOFRAMEWORK_1_0
            if(arrayType.GenericTypeArguments.Length > 0)
            {
                elementType = arrayType.GenericTypeArguments[0];
            }
#endif
            return length > 0 ? ReadArray(reader, length, elementType ?? throw ExceptionUtility.ConverterNotFound(typeof(object))) : null;
        }

        private static IList ReadArray(IMessagePackReader reader, int length, Type elementType)
        {
            var targetArray = (IList)Array.CreateInstance(elementType, length);
            if (elementType.IsArray)
            {
                for (var i = 0; i < length; i++)
                {
                    targetArray[i] = Read(reader, elementType);
                }
            }
            else
            {
                var converter = ConverterContext.GetConverter(elementType);
                if (converter != null)
                {
                    for (var i = 0; i < length; i++)
                    {
                        targetArray[i] = converter.Read(reader);
                    }
                }
                else
                {
                    for (var i = 0; i < length; i++)
                    {
                        targetArray[i] = ConverterContext.DeserializeObject(elementType, reader);
                    }
                }
            }

            return targetArray;
        }

        void IConverter.Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            Write((Array?)value, writer);
        }

        object? IConverter.Read([NotNull] IMessagePackReader reader)
        {
            return Read(reader);
        }
    }
}
