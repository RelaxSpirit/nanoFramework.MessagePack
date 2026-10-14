// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Collections;
using System.Runtime.CompilerServices;
using MessagePack.Extensions;
using nanoFramework.MessagePack.Converters;
using nanoFramework.MessagePack.Dto;
using nanoFramework.MessagePack.Exceptions;
using nanoFramework.MessagePack.Extensions;
using nanoFramework.MessagePack.Stream;
using nanoFramework.MessagePack.Utility;

namespace nanoFramework.MessagePack
{
#nullable enable
    internal delegate object? DataTypesActionDelegate(byte dataType, IMessagePackReader reader);

    /// <summary>
    /// Context by serialization/deserialization
    /// </summary>
    public static class ConverterContext
    {
        private static readonly Type[] s_emptyTypes = new Type[0];
        private static readonly NullConverter s_nullConverter = new();
        private static IConverter? s_longConverter = new LongConverter();
        private static IConverter? s_stringConverter = new StringConverter();
        private static IConverter? s_binaryConverter = new BinaryConverter();
        private static IConverter? s_ushortConverter = new UshortConverter();
        private static readonly DataTypesActionDelegate[] s_highBitsDataTypesActions = new DataTypesActionDelegate[]
        {
            //// 0x8... => DataTypes.FixMap
            (dataType, reader) => MapConverter.ReadMap(reader, dataType - (int)DataTypes.FixMap),
            //// 0x9... => DataTypes.FixArray
            (dataType, reader) => ArrayListConverter.ReadArrayList(reader, dataType - (int)DataTypes.FixArray),
            //// 0xA... => DataTypes.FixStr
            (dataType, reader) => Converters.StringConverter.ReadString(reader, dataType - (uint)DataTypes.FixStr),
            //// 0xB... <= DataTypes.FixStr
            (dataType, reader) => Converters.StringConverter.ReadString(reader, dataType - (uint)DataTypes.FixStr),
            //// 0xC... => DataTypes.Null, DataTypes.False, .....
            (dataType, reader) => ReadCObject(dataType, reader),
            //// 0xD... => DataTypes.Int8, DataTypes.Int16, .....
            (dataType, reader) => ReadDObject(dataType, reader),
            //// 0xE... => DataTypes.NegativeFixNum
            (dataType, reader) => (sbyte)(dataType - 1 - byte.MaxValue),
            //// 0xF... <= DataTypes.NegativeFixNum
            (dataType, reader) => (sbyte)(dataType - 1 - byte.MaxValue)
        };
        private static readonly TypeValueDictionary s_mappingDictionary = new();
        private static readonly TypeValueDictionary s_conversionTable = new()
        {
            {typeof(string), s_stringConverter },
            {typeof(Hashtable), new MapConverter()},
            {typeof(IDictionary), new MapConverter() },
            {typeof(long),  s_longConverter},
            {typeof(ulong),  new UlongConverter()},
            {typeof(byte),  new ByteConverter()},
            {typeof(int), new IntConverter()},
            {typeof(uint), new UintConverter()},
            {typeof(ArrayList), new ArrayListConverter()},
            {typeof(short), new ShortConverter()},
            {typeof(ushort), s_ushortConverter},
            {typeof(sbyte), new SbyteConverter()},
            {typeof(float), new FloatConverter()},
            {typeof(double), new DoubleConverter()},
            {typeof(bool), new BoolConverter()},
            {typeof(char), new CharConverter()},
            {typeof(Guid),new GuidConverter()},
            {typeof(DateTime), new DateTimeConverter()},
            {typeof(TimeSpan),  new TimeSpanConverter()},
            {typeof(byte[]), s_binaryConverter},
            {typeof(int[]), new SimpleArrayConverter(typeof(int))},
            {typeof(uint[]), new SimpleArrayConverter(typeof(uint))},
            {typeof(long[]), new SimpleArrayConverter(typeof(long))},
            {typeof(ulong[]), new SimpleArrayConverter(typeof(ulong))},
            {typeof(float[]), new SimpleArrayConverter(typeof(float))},
            {typeof(double[]), new SimpleArrayConverter(typeof(double))},
            {typeof(char[]), new SimpleArrayConverter(typeof(char))},
            {typeof(string[]), new SimpleArrayConverter(typeof(string))},
            {typeof(Guid[]), new SimpleArrayConverter(typeof(Guid))},
            {typeof(sbyte[]), new SimpleArrayConverter(typeof(sbyte))},
            {typeof(DateTime[]), new SimpleArrayConverter(typeof(DateTime))},
            {typeof(TimeSpan[]), new SimpleArrayConverter(typeof(TimeSpan))},
            {typeof(bool[]), new SimpleArrayConverter(typeof(bool))},
            {typeof(short[]), new SimpleArrayConverter(typeof(short))},
            {typeof(ushort[]), new SimpleArrayConverter(typeof(ushort))}
#if !NANOFRAMEWORK_1_0
            ,{typeof(short?), new NullableConverter<short>() },
            { typeof(ushort?), new NullableConverter<ushort>() },
            { typeof(int?), new NullableConverter<int>() },
            { typeof(uint?), new NullableConverter<uint>() },
            { typeof(long?), new NullableConverter<long>() },
            { typeof(ulong?), new NullableConverter<ulong>() },
            { typeof(byte?), new NullableConverter<byte>() },
            { typeof(sbyte?), new NullableConverter<sbyte>() },
            { typeof(float?), new NullableConverter<float>() },
            { typeof(double?), new NullableConverter<double>() },
            { typeof(bool?), new NullableConverter<bool>() },
            { typeof(TimeSpan?), new NullableConverter<TimeSpan>() },
            { typeof(DateTime?), new NullableConverter<DateTime>() },
            { typeof(char?), new NullableConverter<char>() },
            { typeof(Guid?), new NullableConverter<Guid>() },
#endif
        };

        /// <summary>
        /// <see langword="null"/> value converter.
        /// </summary>
        public static IConverter NullConverter => s_nullConverter;

        /// <summary>
        /// <see langword="long"/> value converter.
        /// </summary>
        internal static IConverter LongConverter => s_longConverter ?? throw ExceptionUtility.ConverterNotFound(typeof(long));

        /// <summary>
        /// <see langword="string"/> value converter.
        /// </summary>
        internal static IConverter StringConverter => s_stringConverter ?? throw ExceptionUtility.ConverterNotFound(typeof(string));

        /// <summary>
        /// <see langword="byte"/> array converter.
        /// </summary>
        internal static IConverter BinaryConverter => s_binaryConverter ?? throw ExceptionUtility.ConverterNotFound(typeof(byte[]));

        /// <summary>
        /// <see langword="ushort"/> value converter.
        /// </summary>
        internal static IConverter UshortConverter => s_ushortConverter ?? throw ExceptionUtility.ConverterNotFound(typeof(ushort));

        /// <summary>
        /// Adds new converter to collection to support more types.
        /// </summary>
        /// <param name="type">Type of object.</param>
        /// <param name="converter">Converter instance which will be used to convert <paramref name="type"/>.</param>
        /// <exception cref="NotSupportedException">Converter by type <see cref="object"/> not support in convertors table.</exception>
        public static void Add(Type type, IConverter converter)
        {
            if (type == typeof(object))
            {
                throw new NotSupportedException();
            }

            s_conversionTable.Add(type, converter);

            UpdateFrequentlyUsedConverter(type, converter);
        }

        /// <summary>
        /// Remove existing type converter.
        /// </summary>
        /// <param name="type">Type of object.</param>
        public static void Remove(Type type)
        {
            s_conversionTable.Remove(type);

            UpdateFrequentlyUsedConverter(type, null);
        }

        /// <summary>
        /// Remove and then adds converter for given type.
        /// </summary>
        /// <param name="type">Type of object.</param>
        /// <param name="converter">Converter instance which will be used to convert <paramref name="type"/>.</param>
        public static void Replace(Type type, IConverter converter)
        {
            Remove(type);
            Add(type, converter);
        }
#nullable enable
        /// <summary>
        /// Return converter by type.
        /// </summary>
        /// <param name="type">Type object from converter.</param>
        /// <returns>Converter interface <see cref="IConverter"/> or null.</returns>
        /// <exception cref="ConverterNotFoundException">If object type is <see cref="object"/>.</exception>
        public static IConverter? GetConverter(Type type)
        {
            if (type == typeof(object))
            {
                throw ExceptionUtility.ConverterNotFound(type);
            }

            return (IConverter?)s_conversionTable[type];
        }

        internal static Hashtable GetMappingsValues(Type targetType, object value)
        {
            MemberMapping[] memberMappings = GetMemberMapping(targetType);

            Hashtable result = new();

            for (int i = 0; i < memberMappings.Length; i++)
            {
                if (memberMappings[i] is MemberMapping memberMapping)
                {
                    if (memberMapping.TryGetValue(value, out object? memberValue) && memberValue != null)
                    {
                        result.Add(memberMapping.Name, memberValue);
                    }
                }
            }
            return result;
        }

        internal static void SetMappingsValues(Type targetType, object targetObject, Hashtable objectValuesMap)
        {
            MemberMapping[] memberMappings = GetMemberMapping(targetType);

            Type? preMemberValueMapType = null;
            IConverter? converter = null;

            for (int i = 0; i < memberMappings.Length; i++)
            {
                if (memberMappings[i] is MemberMapping memberMapping)
                {
                    if (objectValuesMap[memberMapping.Name] is ArraySegment memberMapToken)
                    {
                        Type memberValueMapType = memberMapping.MemberType;
                        if (memberValueMapType != preMemberValueMapType)
                        {
                            converter = GetConverter(memberValueMapType);
                            preMemberValueMapType = memberValueMapType;
                        }

                        if (converter != null)
                        {
                            memberMapping.SetValue(targetObject, converter.Read(memberMapToken));
                        }
                        else
                        {
#if !NANOFRAMEWORK_1_0
                            if (memberValueMapType.IsEnum)
                            {
                                DataTypes dataType = memberMapToken.ReadDataType();

                                if (dataType == DataTypes.Null)
                                    continue;

                                if (dataType.GetHighBits(3) == DataTypes.FixStr.GetHighBits(3) || dataType == DataTypes.Str8 || dataType == DataTypes.Str16 || dataType == DataTypes.Str32)
                                {
                                    var stringEnumConverter = new FromStringEnumConverter(memberValueMapType);
                                    Replace(memberValueMapType, new FromStringEnumConverter(memberValueMapType));

                                    memberMapToken.Seek(0, SeekOrigin.Begin);
                                    memberMapping.SetValue(targetObject, stringEnumConverter.Read(memberMapToken)!);
                                    continue;
                                }

                                memberMapToken.Seek(0, SeekOrigin.Begin);
                            }
#endif
                            memberMapping.SetValue(targetObject, DeserializeObject(memberValueMapType, memberMapToken));
                        }
                    }
                }
            }
        }

        internal static object CreateInstance(Type targetType)
        {
            var constructor = targetType.GetConstructor(s_emptyTypes) ?? throw new Exception($"Target type {targetType?.FullName} does not have a parameterless constructor");
            return constructor.Invoke(null);
        }

        internal static MemberMapping[] GetMemberMapping(Type targetType)
        {
            var cached = s_mappingDictionary[targetType];
            if (cached is MemberMapping[] memberMappings)
            {
                return memberMappings;
            }
            else
            {
                var mappings = new ArrayList();

                FieldUtility.Map(targetType, mappings);
                PropertyUtility.Map(targetType, mappings);

                memberMappings = (MemberMapping[])mappings.ToArray(typeof(MemberMapping));

                s_mappingDictionary[targetType] = memberMappings;
                return memberMappings;
            }
        }

        internal static void SerializeObject(Type type, object value, IMessagePackWriter writer)
        {
            if (value is IDictionary dictionary)
            {
                MapConverter.Write(dictionary, writer);
                return;
            }

            if (value is Hashtable hashtable)
            {
                MapConverter.Write(hashtable, writer);
                return;
            }

            if (type.IsArray || value is IList)
            {
#if !NANOFRAMEWORK_1_0
                if(type.GenericTypeArguments.Length > 0)
                {
                    SimpleArrayConverter.Write((IList)value, type.GenericTypeArguments[0], writer);
                    return;
                }
#endif
                SimpleArrayConverter.Write((IList)value, null, writer);
                return;
            }

#if !NANOFRAMEWORK_1_0
            if (type.IsEnum)
            {
                IConverter? converter = GetConverter(type);

                if (converter == null)
                {
                    converter = new FromStringEnumConverter(type);
                    Replace(type, converter);
                }

                converter.Write(value, writer);
                return;
            }
#endif
            var objectMap = GetMappingsValues(type, value);
            MapConverter.Write(objectMap, writer);

        }

        internal static object? DeserializeObject(Type type, IMessagePackReader reader)
        {
            if (type.Name == typeof(Hashtable).Name || type.IsImplementInterface(typeof(IDictionary)) || type.IsGenericDictionary())
            {
#if NANOFRAMEWORK_1_0
                return MapConverter.Read(reader);
#else
                if (type.Name == typeof(Hashtable).Name)
                {
                    return MapConverter.Read(reader);
                }
                else
                {
                    object? resultDictionary;
                    if (type.IsInterface)
                    {
                        var genericType = typeof(Dictionary<,>).MakeGenericType(type.GenericTypeArguments);
                        resultDictionary = Activator.CreateInstance(genericType);
                    }
                    else
                    {
                        resultDictionary = Activator.CreateInstance(type);
                    }

                    MapConverter.FillDictionary(reader, (IDictionary)resultDictionary!);

                    return resultDictionary;
                }
#endif
            }

            if (type.IsEnum || type.IsNullableGenericEnum())
            {
#if !NANOFRAMEWORK_1_0
                Type enumType = type.IsEnum ? type : type.GenericTypeArguments[0];

                IConverter? converter = GetConverter(enumType);

                if (converter == null)
                {
                    converter = new FromStringEnumConverter(enumType);
                    Replace(type, converter);
                }

                return converter.Read(reader);
#else
                return GetConverter(typeof(int))?.Read(reader);
#endif
            }

            if (type.IsArray || type.IsImplementInterface(typeof(IList)) || type.IsImplementInterface(typeof(ICollection)) || type.IsGenericArray())
            {
                IList? resultArray = SimpleArrayConverter.Read(reader, type);

                if (resultArray == null)
                    return null;

                if (type.IsArray)
                {
                    return resultArray;
                }
                else
                {
                    if (type.IsInterface)
                    {
#if NANOFRAMEWORK_1_0
                        return resultArray;
#else
                        Type genericType = typeof(List<>).MakeGenericType(type.GenericTypeArguments);
                        return Activator.CreateInstance(genericType, new object[] { resultArray! });
#endif
                    }
                    else
                    {
                        var constructor = type.GetConstructor(new Type[] { resultArray!.GetType() }) ?? throw new Exception($"Target type {type.FullName} does not have a parameterless constructor");
                        return constructor.Invoke(new object[] { resultArray });
                    }
                }
            }

            try
            {
                Hashtable? objectMap = reader.GetMessagePackObjectTokens();

                if (objectMap is Hashtable targetObjectMap)
                {
                    var targetObject = CreateInstance(type);
                    SetMappingsValues(type, targetObject, targetObjectMap);
                    return targetObject;
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
            catch (Exception ex)
            {
#if !NANOFRAMEWORK_1_0
                throw new SerializationException($"Type {type.Name} can not by deserialize.", ex);
#else
                throw new SerializationException(type.Name, ex);
#endif
            }
        }

        internal static object? GetObjectByDataType(IMessagePackReader reader)
        {
            DataTypes type = reader.ReadDataType();
            if (type.GetHighBits(1) == DataTypes.PositiveFixNum.GetHighBits(1))
            {
                return (byte)type;
            }
            else
            {
                int highIndex = type.GetHighBits(4) - DataTypes.FixMap.GetHighBits(4);
                return s_highBitsDataTypesActions[highIndex].Invoke((byte)type, reader);
            }
        }

        private static object? ReadCObject(byte dataType, IMessagePackReader reader)
        {
            DataTypes type = (DataTypes)dataType;
            return type switch
            {
                DataTypes.Null => null,
                DataTypes.False => false,
                DataTypes.True => true,
                DataTypes.Bin8 => reader.ReadBytes(reader.ReadByte()),
                DataTypes.Bin16 => reader.ReadBytes(NumberConverterHelper.ReadUInt16(reader)),
                DataTypes.Bin32 => reader.ReadBytes(NumberConverterHelper.ReadUInt32(reader)),
                DataTypes.Timestamp96 => ReadDateTimeExt(type, reader),
                DataTypes.Single => NumberConverterHelper.ReadFloat(reader),
                DataTypes.Double => NumberConverterHelper.ReadDouble(reader),
                DataTypes.UInt8 => reader.ReadByte(),
                DataTypes.UInt16 => NumberConverterHelper.ReadUInt16(reader),
                DataTypes.UInt32 => NumberConverterHelper.ReadUInt32(reader),
                DataTypes.UInt64 => NumberConverterHelper.ReadUInt64(reader),
                _ => throw ExceptionUtility.BadTypeException((DataTypes)dataType, DataTypes.Null, DataTypes.False, DataTypes.True, DataTypes.Bin8, DataTypes.Bin16, DataTypes.Bin32, DataTypes.Timestamp96, DataTypes.Single, DataTypes.Double, DataTypes.UInt8, DataTypes.UInt16, DataTypes.UInt32, DataTypes.UInt64)
            };
        }

        private static object? ReadDObject(byte dataType, IMessagePackReader reader)
        {
            DataTypes type = (DataTypes)dataType;
            return type switch
            {
                DataTypes.Int8 => (byte)NumberConverterHelper.ReadInt8(reader),
                DataTypes.Int16 => NumberConverterHelper.ReadInt16(reader),
                DataTypes.Int32 => NumberConverterHelper.ReadInt32(reader),
                DataTypes.Int64 => NumberConverterHelper.ReadInt64(reader),
                DataTypes.Timestamp32 => ReadDateTimeExt(type, reader),
                DataTypes.Timestamp64 => ReadDateTimeExt(type, reader),
                DataTypes.Str8 => Converters.StringConverter.ReadString(reader, reader.ReadByte()),
                DataTypes.Str16 => Converters.StringConverter.ReadString(reader, NumberConverterHelper.ReadUInt16(reader)),
                DataTypes.Str32 => Converters.StringConverter.ReadString(reader, NumberConverterHelper.ReadUInt32(reader)),
                DataTypes.Array16 => ArrayListConverter.ReadArrayList(reader, NumberConverterHelper.ReadUInt16(reader)),
                DataTypes.Array32 => ArrayListConverter.ReadArrayList(reader, (int)NumberConverterHelper.ReadUInt32(reader)),
                DataTypes.Map16 => MapConverter.ReadMap(reader, NumberConverterHelper.ReadUInt16(reader)),
                DataTypes.Map32 => MapConverter.ReadMap(reader, NumberConverterHelper.ReadInt32(reader)),
                _ => throw ExceptionUtility.BadTypeException((DataTypes)dataType, DataTypes.Int8, DataTypes.Int16, DataTypes.Int32, DataTypes.Int64, DataTypes.Timestamp32, DataTypes.Timestamp64, DataTypes.Str8, DataTypes.Str16, DataTypes.Str32, DataTypes.Array16, DataTypes.Array32, DataTypes.Map16, DataTypes.Map32)
            };
        }

        private static DateTime ReadDateTimeExt(DataTypes type, IMessagePackReader reader)
        {
            uint nanoSeconds = 0;
            sbyte extType = NumberConverterHelper.ReadInt8(reader);
            long seconds;
            if (extType == -1)
            {
                if (type == DataTypes.Timestamp32)
                {
                    seconds = NumberConverterHelper.ReadUInt32(reader);
                }
                else
                {
                    nanoSeconds = NumberConverterHelper.ReadUInt32(reader);
                    seconds = NumberConverterHelper.ReadUInt32(reader);
                }
            }
            else if (extType == 12)
            {
                extType = NumberConverterHelper.ReadInt8(reader);
                if (extType == -1)
                {
                    nanoSeconds = NumberConverterHelper.ReadUInt32(reader);
                    seconds = NumberConverterHelper.ReadInt64(reader);
                }
                else
                {
                    throw ExceptionUtility.BadTypeException(type, DataTypes.Timestamp32, DataTypes.Timestamp64, DataTypes.Timestamp96);
                }
            }
            else
            {
                throw ExceptionUtility.BadTypeException(type, DataTypes.Timestamp32, DataTypes.Timestamp64, DataTypes.Timestamp96);
            }

            return DateTime.UnixEpoch.AddSeconds(seconds).AddMilliseconds(nanoSeconds / 1000.0);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void UpdateFrequentlyUsedConverter(Type type, IConverter? converter)
        {
            if (type == typeof(string))
            {
                s_stringConverter = converter;
            }
            else if (type == typeof(long))
            {
                s_longConverter = converter;
            }
            else if (type == typeof(ushort))
            {
                s_ushortConverter = converter;
            }
            else if (type == typeof(byte[]))
            {
                s_binaryConverter = converter;
            }
        }
    }
}
