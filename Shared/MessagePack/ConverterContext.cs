// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
#if NANOFRAMEWORK_1_0
using System.Diagnostics;
#endif
using System.Runtime.CompilerServices;
using nanoFramework.MessagePack.Converters;
using nanoFramework.MessagePack.Dto;
using nanoFramework.MessagePack.Exceptions;
using nanoFramework.MessagePack.Extensions;
using nanoFramework.MessagePack.Stream;
using nanoFramework.MessagePack.Utility;
using MessagePack.Extensions;


#if !NANOFRAMEWORK_1_0
using System.Collections.Concurrent;
using System.Collections.Generic;
#endif

namespace nanoFramework.MessagePack
{
    /// <summary>
    /// Context by serialization/deserialization
    /// </summary>
    public static class ConverterContext
    {
        private static readonly Type[] s_emptyTypes = new Type[0];
        private static readonly NullConverter s_nullConverter = new NullConverter();
        private static readonly MapConverter s_mapConverter = new MapConverter();
        private static readonly ArrayConverter s_arrayConverter = new ArrayConverter();

#if NANOFRAMEWORK_1_0
        private static readonly Hashtable s_mappingDictionary = new();
#else
        private static readonly ConcurrentDictionary<Type, MemberMapping[]> s_mappingDictionary = new ConcurrentDictionary<Type, MemberMapping[]>();
#endif

        private static readonly Hashtable s_conversionTable = new Hashtable()
        {
            { typeof(IDictionary).FullName!, s_mapConverter },
            { typeof(Hashtable).FullName!, s_mapConverter },
            { typeof(ArrayList).FullName!, new ArrayListConverter() },
            { typeof(short).FullName!, new ShortConverter() },
            { typeof(ushort).FullName!, new UshortConverter() },
            { typeof(int).FullName!, new IntConverter() },
            { typeof(uint).FullName!, new UintConverter() },
            { typeof(long).FullName!, new LongConverter() },
            { typeof(ulong).FullName!, new UlongConverter() },
            { typeof(byte).FullName!, new ByteConverter() },
            { typeof(sbyte).FullName!, new SbyteConverter() },
            { typeof(float).FullName!, new FloatConverter() },
            { typeof(double).FullName!, new DoubleConverter() },
            { typeof(bool).FullName!, new BoolConverter() },
            { typeof(string).FullName!, new StringConverter() },
            { typeof(TimeSpan).FullName!, new TimeSpanConverter() },
            { typeof(DateTime).FullName!, new DateTimeConverter() },
            { typeof(char).FullName!, new CharConverter() },
            { typeof(Guid).FullName!, new GuidConverter() },
            { typeof(byte[]).FullName!, new BinaryConverter() },
            { typeof(int[]).FullName!, new SimpleArrayConverter(typeof(int)) },
            { typeof(uint[]).FullName!, new SimpleArrayConverter(typeof(uint)) },
            { typeof(long[]).FullName!, new SimpleArrayConverter(typeof(long)) },
            { typeof(ulong[]).FullName!, new SimpleArrayConverter(typeof(ulong)) },
            { typeof(float[]).FullName!, new SimpleArrayConverter(typeof(float)) },
            { typeof(double[]).FullName!, new SimpleArrayConverter(typeof(double)) },
            { typeof(char[]).FullName!, new SimpleArrayConverter(typeof(char)) },
            { typeof(string[]).FullName!, new SimpleArrayConverter(typeof(string)) },
            { typeof(Guid[]).FullName!, new SimpleArrayConverter(typeof(Guid)) },
            { typeof(sbyte[]).FullName!, new SimpleArrayConverter(typeof(sbyte)) },
            { typeof(DateTime[]).FullName!, new SimpleArrayConverter(typeof(DateTime)) },
            { typeof(TimeSpan[]).FullName!, new SimpleArrayConverter(typeof(TimeSpan)) },
            { typeof(bool[]).FullName!, new SimpleArrayConverter(typeof(bool)) },
            { typeof(short[]).FullName!, new SimpleArrayConverter(typeof(short)) },
            { typeof(ushort[]).FullName!, new SimpleArrayConverter(typeof(ushort)) }
#if !NANOFRAMEWORK_1_0
            ,{ typeof(short?).FullName!, new NullableConverter<short>() },
            { typeof(ushort?).FullName!, new NullableConverter<ushort>() },
            { typeof(int?).FullName!, new NullableConverter<int>() },
            { typeof(uint?).FullName!, new NullableConverter<uint>() },
            { typeof(long?).FullName!, new NullableConverter<long>() },
            { typeof(ulong?).FullName!, new NullableConverter<ulong>() },
            { typeof(byte?).FullName!, new NullableConverter<byte>() },
            { typeof(sbyte?).FullName!, new NullableConverter<sbyte>() },
            { typeof(float?).FullName!, new NullableConverter<float>() },
            { typeof(double?).FullName!, new NullableConverter<double>() },
            { typeof(bool?).FullName!, new NullableConverter<bool>() },
            { typeof(TimeSpan?).FullName!, new NullableConverter<TimeSpan>() },
            { typeof(DateTime?).FullName!, new NullableConverter<DateTime>() },
            { typeof(char?).FullName!, new NullableConverter<char>() },
            { typeof(Guid?).FullName!, new NullableConverter<Guid>() },
#endif
        };

        /// <summary>
        /// Null value converter.
        /// </summary>
        public static IConverter NullConverter => s_nullConverter;

        /// <summary>
        /// Adds new converter to collection to support more types.
        /// </summary>
        /// <param name="type">Type of object.</param>
        /// <param name="converter">Converter instance which will be used to convert <paramref name="type"/>.</param>
        /// <exception cref="NotSupportedException">Converter by type <see cref="object"/> not support in convertors table.</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Add(Type type, IConverter converter)
        {
            if (type == typeof(object))
            {
                throw new NotSupportedException();
            }

            s_conversionTable.Add(type.FullName!, converter);
        }

        /// <summary>
        /// Remove existing type converter.
        /// </summary>
        /// <param name="type">Type of object.</param>
        public static void Remove(Type type)
        {
            s_conversionTable.Remove(type.FullName!);
        }

        /// <summary>
        /// Remove and then adds converter for given type.
        /// </summary>
        /// <param name="type">Type of object.</param>
        /// <param name="converter">Converter instance which will be used to convert <paramref name="type"/>.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Replace(Type type, IConverter converter)
        {
            Remove(type);
            Add(type, converter);
        }

        /// <summary>
        /// Return converter by type.
        /// </summary>
        /// <param name="type">Type object from converter.</param>
        /// <returns>Converter interface <see cref="IConverter"/> or null.</returns>
        /// <exception cref="ConverterNotFoundException">If object type is <see cref="object"/>.</exception>
        public static IConverter GetConverter(Type type)
        {
            if (type == typeof(object))
            {
                throw ExceptionUtility.ConverterNotFound(type);
            }

            if (s_conversionTable.Contains(type.FullName!))
            {
                return (IConverter)s_conversionTable[type.FullName!]!;
            }

            return null!;
        }

        internal static Hashtable GetMappingsValues(Type targetType, object value)
        {
            var memberMappings = GetMemberMapping(targetType);

            Hashtable result = new Hashtable();

            foreach (var memberMapping in memberMappings)
            {
#if NANOFRAMEWORK_1_0
                if (!memberMapping.OriginalName!.StartsWith(MemberMapping.SET_))
                {
#endif
                    if (memberMapping.TryGetValue(value, out var memberValue) && memberValue != null)
                    {
                        result.Add(memberMapping.Name!, memberValue);
                    }
#if NANOFRAMEWORK_1_0
                }
#endif
            }
            return result;
        }

        internal static void SetMappingsValues(Type targetType, object targetObject, Hashtable objectValuesMap)
        {
            var memberMappings = GetMemberMapping(targetType);

            foreach (var memberMapping in memberMappings)
            {
#if NANOFRAMEWORK_1_0
                if (!memberMapping.OriginalName!.StartsWith(MemberMapping.GET_))
                {
#endif
                if (objectValuesMap.Contains(memberMapping.Name!))
                {
                    var memberMpToken = (ArraySegment)objectValuesMap[memberMapping.Name!]!;
                    if (memberMpToken != null)
                    {
                        try
                        {
                            var memberValueMapType = memberMapping.GetMemberType();
                            var converter = GetConverter(memberValueMapType!);
                            if (converter != null)
                            {
                                memberMapping.SetValue(targetObject, converter.Read(memberMpToken)!);
                            }
                            else
                            {
                                if (memberValueMapType!.IsArray)
                                {
                                    memberMapping.SetValue(targetObject, ArrayConverter.Read(memberMpToken, memberValueMapType)!);
                                }
                                else
                                {
#if !NANOFRAMEWORK_1_0
                                    if (memberValueMapType.IsEnum)
                                    {
                                        var dataType = memberMpToken.ReadDataType();

                                        if (dataType == DataTypes.Null)
                                            continue;

                                        if (dataType == DataTypes.FixStr || dataType == DataTypes.Str8 || dataType == DataTypes.Str16 || dataType == DataTypes.Str32)
                                        {
                                            var stringEnumConverter = new FromStringEnumConverter(memberValueMapType);
                                            Replace(memberValueMapType, new FromStringEnumConverter(memberValueMapType));

                                            memberMpToken.Seek(0, System.IO.SeekOrigin.Begin);
                                            memberMapping.SetValue(targetObject, stringEnumConverter.Read(memberMpToken)!);
                                            continue;
                                        }

                                        memberMpToken.Seek(0, System.IO.SeekOrigin.Begin);
                                    }
#endif
                                    memberMapping.SetValue(targetObject, DeserializeObject(memberValueMapType!, memberMpToken)!);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new SerializationException($"Error deserialize member name: '{memberMapping.Name}' and type: '{memberMapping.GetMemberType()}'\n{ex}");
                        }
                    }
#if NANOFRAMEWORK_1_0
                }
#endif
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
#if NANOFRAMEWORK_1_0
            var cached = s_mappingDictionary[targetType];
            if (cached is not MemberMapping[] memberMappings)
            {
#else
            if (!s_mappingDictionary.TryGetValue(targetType, out var memberMappings))
            {
#endif
                var mappings = new ArrayList();

                FieldUtility.Map(targetType, mappings);
                PropertyUtility.Map(targetType, mappings);

                memberMappings = (MemberMapping[])mappings.ToArray(typeof(MemberMapping));

#if NANOFRAMEWORK_1_0
                ThreadSafeAddItemCache(s_mappingDictionary, targetType, memberMappings);
#else
                s_mappingDictionary.TryAdd(targetType, memberMappings);
#endif
            }

            return memberMappings;
        }

        internal static void SerializeObject(Type type, object value, IMessagePackWriter writer)
        {
            if (value is IDictionary || value is Hashtable)
            {
                s_mapConverter.Write(value, writer);
                return;
            }

            if (type.IsArray || value is IList || value is ICollection)
            {
                s_arrayConverter.Write(value, writer);
                return;
            }

#if !NANOFRAMEWORK_1_0
            if (type.IsEnum)
            {
                IConverter converter = GetConverter(type);

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
            s_mapConverter.Write(objectMap, writer);

        }
#nullable enable
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

            if(type.IsEnum || type.IsNullableGenericEnum())
            {
#if !NANOFRAMEWORK_1_0
                var enumType = type.IsEnum ? type : type.GenericTypeArguments[0];

                IConverter converter = GetConverter(enumType);

                if (converter == null)
                {
                    converter = new FromStringEnumConverter(enumType);
                    Replace(type, converter);
                }

                return converter.Read(reader);
#else
                return GetConverter(typeof(int)).Read(reader);
#endif
            }

            if (type.IsArray || type.IsImplementInterface(typeof(IList)) || type.IsImplementInterface(typeof(ICollection)) || type.IsGenericArray())
            {
                var resultArray = ArrayConverter.Read(reader, type);
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
                        var genericType = typeof(List<>).MakeGenericType(type.GenericTypeArguments);
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
                var objectMap = reader.GetMassagePackObjectTokens();

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
            catch(Exception ex)
            {
                throw new SerializationException($"Type {type.Name} can not by deserialize.\n{ex}");
            }
        }

        internal static object? GetObjectByDataType(IMessagePackReader reader)
        {
            var type = reader.ReadDataType();

            return type switch
            {
                DataTypes.Null => null,
                DataTypes.True => true,
                DataTypes.False => false,
                DataTypes.Double => NumberConverterHelper.ReadDouble(reader),
                DataTypes.Single => NumberConverterHelper.ReadFloat(reader),
                DataTypes.Str8 => StringConverter.ReadString(reader, NumberConverterHelper.ReadUInt8(reader)),
                DataTypes.Str16 => StringConverter.ReadString(reader, NumberConverterHelper.ReadUInt16(reader)),
                DataTypes.Str32 => StringConverter.ReadString(reader, NumberConverterHelper.ReadUInt32(reader)),
                DataTypes.UInt8 => NumberConverterHelper.ReadUInt8(reader),
                DataTypes.Int8 => (byte)NumberConverterHelper.ReadInt8(reader),
                DataTypes.UInt16 => NumberConverterHelper.ReadUInt16(reader),
                DataTypes.Int16 => NumberConverterHelper.ReadInt16(reader),
                DataTypes.UInt32 => NumberConverterHelper.ReadUInt32(reader),
                DataTypes.Int32 => NumberConverterHelper.ReadInt32(reader),
                DataTypes.UInt64 => NumberConverterHelper.ReadUInt64(reader),
                DataTypes.Int64 => NumberConverterHelper.ReadInt64(reader),
                DataTypes.Bin8 => reader.ReadBytes(NumberConverterHelper.ReadUInt8(reader)),
                DataTypes.Bin16 => reader.ReadBytes(NumberConverterHelper.ReadUInt16(reader)),
                DataTypes.Bin32 => reader.ReadBytes(NumberConverterHelper.ReadUInt32(reader)),
                DataTypes.Timestamp32 => ReadDateTimeExt(type, reader),
                DataTypes.Timestamp64 => ReadDateTimeExt(type, reader),
                DataTypes.Timestamp96 => ReadDateTimeExt(type, reader),
                DataTypes.Array16 => ArrayListConverter.ReadArrayList(reader, NumberConverterHelper.ReadUInt16(reader)),
                DataTypes.Array32 => ArrayListConverter.ReadArrayList(reader, NumberConverterHelper.ReadUInt32(reader)),
                DataTypes.Map16 => MapConverter.ReadMap(reader, NumberConverterHelper.ReadUInt16(reader)),
                DataTypes.Map32 => MapConverter.ReadMap(reader, NumberConverterHelper.ReadUInt32(reader)),
                _ => ReadObject(type, reader),
            };
        }

#if NANOFRAMEWORK_1_0
        private static void ThreadSafeAddItemCache(Hashtable hashtable, object key, object value)
        {
            if (!hashtable.Contains(key))
            {
                lock (s_mappingDictionary)
                {
                    try
                    {
                        if (!hashtable.Contains(key))
                        {
                            hashtable.Add(key, value);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error added key: '{key}', value: '{value}'\r\n{ex}");
                    }
                }
            }
        }
#endif
        private static object ReadObject(DataTypes type, IMessagePackReader reader)
        {
            if (type.GetHighBits(3) == DataTypes.FixStr.GetHighBits(3))
            {
                return StringConverter.ReadString(reader, type - DataTypes.FixStr);
            }

            if (NumberConverterHelper.TryGetFixPositiveNumber(type, out var positive))
            {
                return positive;
            }

            if (NumberConverterHelper.TryGetNegativeNumber(type, out var negative))
            {
                return negative;
            }

            if (type.GetHighBits(4) == DataTypes.FixArray.GetHighBits(4))
            {
                return ArrayListConverter.ReadArrayList(reader, type - DataTypes.FixArray);
            }

            if (type.GetHighBits(4) == DataTypes.FixMap.GetHighBits(4))
            {
                return MapConverter.ReadMap(reader, type - DataTypes.FixMap);
            }

            throw ExceptionUtility.BadTypeException(type);
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
    }
}
