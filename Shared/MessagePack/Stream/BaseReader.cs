// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using nanoFramework.MessagePack.Dto;
using nanoFramework.MessagePack.Extensions;
using nanoFramework.MessagePack.Utility;

namespace nanoFramework.MessagePack.Stream
{
    /// <summary>
    /// Base class for MessagePack reader.
    /// </summary>
    public abstract class BaseReader : IMessagePackReader
    {
        /// <summary>
        /// Read byte from MessagePack.
        /// </summary>
        /// <returns>Read a byte.</returns>
        public abstract byte ReadByte();

        /// <summary>
        /// Read bytes from MessagePack.
        /// </summary>
        /// <param name="length">Length to read.</param>
        /// <returns><see cref="ArraySegment"/> with the bytes read.</returns>
        public abstract ArraySegment ReadBytes(uint length);

        /// <summary>
        /// Moving the current read position in the array.
        /// </summary>
        /// <param name="offset">Offset in bytes.</param>
        /// <param name="origin">Offset reference point.</param>
        public abstract void Seek(long offset, SeekOrigin origin);

        /// <summary>
        /// Read length MessagePack array.
        /// The MessagePacket array type can be <see cref="DataTypes.Array16"/> or <see cref="DataTypes.Array32"/> or <see cref="DataTypes.FixArray"/>.
        /// </summary>
        /// <returns>Length MessagePack array by read for type array <see cref="DataTypes"/>.</returns>
        public uint ReadArrayLength()
        {
            var type = ReadDataType();
            switch (type)
            {
                case DataTypes.Null:
                    return uint.MaxValue;
                case DataTypes.Array16:
                    return NumberConverterHelper.ReadUInt16(this);

                case DataTypes.Array32:
                    return NumberConverterHelper.ReadUInt32(this);
            }

            if (TryGetLengthFromFixArray(type, out var length))
            {
                return length;
            }

            throw ExceptionUtility.BadTypeException(type, DataTypes.Array16, DataTypes.Array32, DataTypes.FixArray, DataTypes.Null);
        }

        /// <summary>
        /// Read length MessagePack map.
        /// The MessagePacket map type can be <see cref="DataTypes.Map16"/> or <see cref="DataTypes.Map32"/> or <see cref="DataTypes.FixMap"/>.
        /// </summary>
        /// <returns>Count elements in map by read for type map <see cref="DataTypes"/>.</returns>
        public uint ReadMapLength()
        {
            var type = ReadDataType();

            switch (type)
            {
                case DataTypes.Null:
                    return uint.MaxValue;
                case DataTypes.Map16:
                    return NumberConverterHelper.ReadUInt16(this);

                case DataTypes.Map32:
                    return NumberConverterHelper.ReadUInt32(this);
            }

            if (TryGetLengthFromFixMap(type, out var length))
            {
                return length;
            }

            throw ExceptionUtility.BadTypeException(type, DataTypes.Map16, DataTypes.Map32, DataTypes.FixMap, DataTypes.Null);
        }

        /// <summary>
        /// Read MessagePack data type <see cref="DataTypes"/>.
        /// </summary>
        /// <returns>MessagePack data type <see cref="DataTypes"/>.</returns>
        public virtual DataTypes ReadDataType()
        {
            return (DataTypes)ReadByte();
        }

        /// <summary>
        /// Skip MessagePack item in byte array.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Unknown <see cref="DataTypes"/> value.</exception>
        public void SkipToken()
        {
            var dataType = ReadDataType();

            switch (dataType)
            {
                case DataTypes.Null:
                case DataTypes.False:
                case DataTypes.True:
                    return;
                case DataTypes.UInt8:
                case DataTypes.Int8:
                    SkipBytes(1);
                    return;
                case DataTypes.UInt16:
                case DataTypes.Int16:
                    SkipBytes(2);
                    return;
                case DataTypes.UInt32:
                case DataTypes.Int32:
                case DataTypes.Single:
                    SkipBytes(4);
                    return;
                case DataTypes.UInt64:
                case DataTypes.Int64:
                case DataTypes.Double:
                    SkipBytes(8);
                    return;
                case DataTypes.Array16:
                    SkipArrayItems(NumberConverterHelper.ReadUInt16(this));
                    return;
                case DataTypes.Array32:
                    SkipArrayItems(NumberConverterHelper.ReadUInt32(this));
                    return;
                case DataTypes.Map16:
                    SkipMapItems(NumberConverterHelper.ReadUInt16(this));
                    return;
                case DataTypes.Map32:
                    SkipMapItems(NumberConverterHelper.ReadUInt32(this));
                    return;
                case DataTypes.Str8:
                    SkipBytes(NumberConverterHelper.ReadUInt8(this));
                    return;
                case DataTypes.Str16:
                    SkipBytes(NumberConverterHelper.ReadUInt16(this));
                    return;
                case DataTypes.Str32:
                    SkipBytes(NumberConverterHelper.ReadUInt32(this));
                    return;
                case DataTypes.Bin8:
                    SkipBytes(NumberConverterHelper.ReadUInt8(this));
                    return;
                case DataTypes.Bin16:
                    SkipBytes(NumberConverterHelper.ReadUInt16(this));
                    return;
                case DataTypes.Bin32:
                    SkipBytes(NumberConverterHelper.ReadUInt32(this));
                    return;
            }

            if (dataType.GetHighBits(1) == DataTypes.PositiveFixNum.GetHighBits(1) ||
                dataType.GetHighBits(3) == DataTypes.NegativeFixNum.GetHighBits(3))
            {
                return;
            }

            if (TryGetLengthFromFixArray(dataType, out var arrayLength))
            {
                SkipArrayItems(arrayLength);
                return;
            }

            if (TryGetLengthFromFixMap(dataType, out var mapLength))
            {
                SkipMapItems(mapLength);
                return;
            }

            if (TryGetLengthFromFixStr(dataType, out var stringLength))
            {
                SkipBytes(stringLength);
                return;
            }

            throw new System.ArgumentOutOfRangeException();
        }

#nullable enable
        /// <summary>
        /// Read MessagePack item in byte array.
        /// </summary>
        /// <returns><see cref="ArraySegment"/> contained MessagePack item bytes.</returns>
        public ArraySegment? ReadToken()
        {
            StartTokenGathering();
            SkipToken();
            var gatheredBytes = StopTokenGathering();
            return gatheredBytes;
        }

        private static bool TryGetLengthFromFixStr(DataTypes type, out uint length)
        {
            length = type - DataTypes.FixStr;
            return type.GetHighBits(3) == DataTypes.FixStr.GetHighBits(3);
        }

        private void SkipMapItems(uint count)
        {
            while (count > 0)
            {
                SkipToken();
                SkipToken();
                count--;
            }
        }

        private void SkipArrayItems(uint count)
        {
            while (count > 0)
            {
                SkipToken();
                count--;
            }
        }

        private void SkipBytes(uint bytesCount)
        {
            Seek(bytesCount, SeekOrigin.Current);
        }

        /// <summary>
        /// Try getting length fixed MessagePack array.
        /// </summary>
        /// <param name="type">MessagePack array type <see cref="DataTypes.FixArray"/>.</param>
        /// <param name="length">Out length MessagePack array.</param>
        /// <returns><see langword="true"/> if luck otherwise <see langword="false"/>.</returns>
        protected static bool TryGetLengthFromFixArray(DataTypes type, out uint length)
        {
            length = type - DataTypes.FixArray;
            return type.GetHighBits(4) == DataTypes.FixArray.GetHighBits(4);
        }

        /// <summary>
        /// Try getting length fixed MessagePack map.
        /// </summary>
        /// <param name="type">MessagePack map type <see cref="DataTypes.FixMap"/>.</param>
        /// <param name="length">Out length MessagePack map.</param>
        /// <returns><see langword="true"/> if luck otherwise <see langword="false"/>.</returns>
        protected static bool TryGetLengthFromFixMap(DataTypes type, out uint length)
        {
            length = type - DataTypes.FixMap;
            return type.GetHighBits(4) == DataTypes.FixMap.GetHighBits(4);
        }

        /// <summary>
        /// Stopping the collection of MessagePack token.
        /// </summary>
        /// <returns><see cref="ArraySegment"/> bytes.</returns>
        protected abstract ArraySegment? StopTokenGathering();

        /// <summary>
        /// Start the collection of MessagePack token.
        /// </summary>
        protected abstract void StartTokenGathering();
    }
}
