﻿using nanoFramework.MessagePack.Stream;
using System;
using System.Collections;
using System.IO;

namespace nanoFramework.MessagePack
{
    public static class MessagePackSerializer
    {
        /// <summary>
        /// Serialize object in to MessagePack byte array
        /// </summary>
        /// <param name="data">Source object</param>
        /// <returns>MessagePack byte array</returns>
        public static byte[] Serialize(object data)
        {
            var memoryStream = new MemoryStream();
            using var writer = new MemoryStreamWriter(memoryStream);

            Serialize(data, writer);

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Serialize object in to MessagePack stream
        /// </summary>
        /// <param name="data">Source object</param>
        /// <param name="stream">Target MessagePack stream</param>
        public static void Serialize(object data, MemoryStream stream)
        {
            var writer = new MemoryStreamWriter(stream);
            Serialize(data, writer);
        }

        /// <summary>
        /// Deserialize MessagePack data in to object
        /// </summary>
        /// <param name="type">Target object type</param>
        /// <param name="data">MessagePack byte array data</param>
        /// <returns>Target object</returns>
#nullable enable
        public static object? Deserialize(Type type, byte[] data)
        {
            var reader = new ByteArrayReader(data);
            return Deserialize(type, reader);
        }

        /// <summary>
        /// Deserialize MessagePack data from stream in to object
        /// </summary>
        /// <param name="type">Target object type</param>
        /// <param name="stream">MessagePack data stream</param>
        /// <returns></returns>
        public static object? Deserialize(Type type, MemoryStream stream)
        {
            using var reader = new MemoryStreamReader(stream);
            return Deserialize(type, reader);

        }

        private static object? Deserialize(Type type, IMessagePackReader reader)
        {
            var converter = ConverterContext.GetConverter(type);
            if (converter != null)
                return converter.Read(reader);
            else
                return ConverterContext.DeserializeObject(type, reader);
        }

        private static void Serialize(object data, IMessagePackWriter writer)
        {
            var type = data.GetType();

            var converter = ConverterContext.GetConverter(type);

            if (converter != null)
                converter.Write(data, writer);
            else
                ConverterContext.SerializeObject(type, data, writer);
        }

    }
}
