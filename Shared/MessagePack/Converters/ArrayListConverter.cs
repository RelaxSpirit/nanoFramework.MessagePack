// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Stream;

namespace nanoFramework.MessagePack.Converters
{
    internal class ArrayListConverter : IConverter
    {
#nullable enable
        private static void Write(ArrayList? value, IMessagePackWriter writer)
        {
            if (value == null)
            {
                ConverterContext.NullConverter.Write(value, writer);
                return;
            }

            writer.WriteArrayHeader((uint)value.Count);

            foreach (var element in value)
            {
                var elementType = element.GetType();
                var elementConverter = ConverterContext.GetConverter(elementType);
                if (elementConverter != null)
                {
                    elementConverter.Write(element, writer);
                }
                else
                {
                    ConverterContext.SerializeObject(elementType, element, writer);
                }
            }
        }

        private static ArrayList? Read(IMessagePackReader reader)
        {
            int length = (int)reader.ReadArrayLength();
            return length > -1 ? ReadArrayList(reader, length) : null;
        }

        internal static ArrayList ReadArrayList(IMessagePackReader reader, int length)
        {
            var array = new ArrayList();

            while (length-- > 0)
            {
                array.Add(ConverterContext.GetObjectByDataType(reader));
            }

            return array;
        }

        public void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            Write((ArrayList?)value, writer);
        }

        object? IConverter.Read([NotNull] IMessagePackReader reader)
        {
            return Read(reader);
        }
    }
}
