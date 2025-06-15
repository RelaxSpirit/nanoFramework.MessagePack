// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
#if !NANOFRAMEWORK_1_0
using System.Linq;
#endif
using nanoFramework.MessagePack.Stream;

namespace nanoFramework.MessagePack.Converters
{
    /// <summary>
    /// Composite array converter
    /// </summary>
    internal class ArrayConverter : IConverter
    {
        private static void Write(IList value, IMessagePackWriter writer)
        {
            if (value == null)
            {
                ConverterContext.NullConverter.Write(value, writer);
                return;
            }

            writer.WriteArrayHeader((uint)value.Count);

            if (value.Count > 0)
            {
                if (value[0] != null)
                {
                    var elementType = value[0]!.GetType();
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
                else
                {
                    ConverterContext.NullConverter.Write(value[0], writer);
                }
            }
        }

#nullable enable
        internal static IList? Read(IMessagePackReader reader, Type arrayType)
        {
            var length = (int)reader.ReadArrayLength();
            return length > -1 ? ReadArray(reader, length, arrayType) : null;
        }

        private static Array ReadArray(IMessagePackReader reader, int length, Type arrayType)
        {
#if NANOFRAMEWORK_1_0
            var elementType = arrayType.GetElementType();
#else
            var elementType = arrayType.GetElementType() ?? arrayType.GenericTypeArguments.FirstOrDefault();
#endif
            var targetArray = (IList)Array.CreateInstance(elementType!, length);

            if (length > 0)
            {
                if (elementType!.IsArray)
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
                            var mpToken = reader.ReadToken();
                            if (mpToken == null || mpToken.ReadDataType() == DataTypes.Null)
                            {
                                targetArray[i] = null;
                            }
                            else
                            {
                                mpToken.Seek(0, System.IO.SeekOrigin.Begin);
                                targetArray[i] = ConverterContext.DeserializeObject(elementType, mpToken);
                            }
                        }
                    }
                }
            }
            return (Array)targetArray;
        }

        public void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            Write((IList)value!, writer);
        }

        object? IConverter.Read([NotNull] IMessagePackReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
