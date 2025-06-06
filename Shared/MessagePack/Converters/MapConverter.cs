// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Stream;

namespace nanoFramework.MessagePack.Converters
{
    internal class MapConverter : IConverter
    {
        private static void Write(IDictionary value, IMessagePackWriter writer)
        {
            if (value == null)
            {
                ConverterContext.NullConverter.Write(value, writer);
                return;
            }

            writer.WriteMapHeader((uint)value.Count);

            foreach (DictionaryEntry element in value)
            {
                var keyType = element.Key.GetType();
                var keyConverter = ConverterContext.GetConverter(keyType);
                if (keyConverter == null)
                {
                    ConverterContext.SerializeObject(keyType, element.Key, writer);
                }
                else
                {
                    keyConverter.Write(element.Key, writer);
                }

                if (element.Value == null)
                {
                    ConverterContext.NullConverter.Write(element.Value, writer);
                }
                else
                {
                    var valueType = element.Value.GetType();
                    var valueConverter = ConverterContext.GetConverter(valueType);
                    if (valueConverter == null)
                    {
                        ConverterContext.SerializeObject(valueType, element.Value, writer);
                    }
                    else
                    {
                        valueConverter.Write(element.Value, writer);
                    }
                }
            }
        }

#nullable enable
        internal static Hashtable? Read(IMessagePackReader reader)
        {
            var length = reader.ReadMapLength();
            return ((long)length) > -1 ? ReadMap(reader, length) : null;
        }

        internal static Hashtable ReadMap(IMessagePackReader reader, uint length)
        {
            var map = new Hashtable();

            for (var i = 0; i < length; i++)
            {
                var key = ConverterContext.GetObjectByDataType(reader);
                var value = ConverterContext.GetObjectByDataType(reader);

                map[key!] = value;
            }

            return map;
        }

#if !NANOFRAMEWORK_1_0
        internal static void FillDictionary(IMessagePackReader reader, IDictionary targetDictionary)
        {
            var length = reader.ReadMapLength();
            if (((long)length) < 1)
            {
                return;
            }
            else
            {
                var dictionaryType = targetDictionary.GetType();

                if (dictionaryType.GenericTypeArguments.Length == 0)
                {
                    dictionaryType = targetDictionary.Keys.GetType();
                }

                while (length-- > 0)
                {
                    targetDictionary[MessagePackSerializer.Deserialize(dictionaryType.GenericTypeArguments[0], (byte[])reader.ReadToken()!)!] = MessagePackSerializer.Deserialize(dictionaryType.GenericTypeArguments[1], (byte[])reader.ReadToken()!);
                }
            }
        }
#endif

        public void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            Write((IDictionary)value!, writer);
        }

        object? IConverter.Read([NotNull] IMessagePackReader reader)
        {
            return Read(reader);
        }
    }
}
