// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using nanoFramework.MessagePack.Extensions;

namespace nanoFramework.MessagePack.Stream
{
    internal class MemoryStreamWriter : IMessagePackWriter, IDisposable
    {
        private readonly MemoryStream _stream;

        public MemoryStreamWriter(MemoryStream stream)
        {
            _stream = stream;
        }

        public void Write(DataTypes dataType)
        {
            _stream.WriteByte((byte)dataType);
        }

        public void Write(byte value)
        {
            _stream.WriteByte(value);
        }

        public void Write(byte[] array)
        {
            _stream.Write(array, 0, array.Length);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public void WriteArrayHeader(uint length)
        {
            if (length <= 15)
            {
                NumberConverterHelper.WriteByteValue((byte)((byte)DataTypes.FixArray + length), this);
                return;
            }

            if (length <= ushort.MaxValue)
            {
                Write(DataTypes.Array16);
                NumberConverterHelper.WriteUShortValue((ushort)length, this);
            }
            else
            {
                Write(DataTypes.Array32);
                NumberConverterHelper.WriteUIntValue(length, this);
            }
        }

        public void WriteMapHeader(uint length)
        {
            if (length <= 15)
            {
                NumberConverterHelper.WriteByteValue((byte)((byte)DataTypes.FixMap + length), this);
                return;
            }

            if (length <= ushort.MaxValue)
            {
                Write(DataTypes.Map16);
                NumberConverterHelper.WriteUShortValue((ushort)length, this);
            }
            else
            {
                Write(DataTypes.Map32);
                NumberConverterHelper.WriteUIntValue(length, this);
            }
        }
    }
}
