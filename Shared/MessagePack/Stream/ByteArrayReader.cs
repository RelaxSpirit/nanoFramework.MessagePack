﻿using MessagePack.Dto;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace nanoFramework.MessagePack.Stream
{
    internal class ByteArrayReader :  BaseReader
    {
        private uint _firstGatheredByte;
        private readonly byte[] _data;

        private uint _offset;

        public ByteArrayReader(byte[] data)
        {
            _data = data;
            _offset = 0;
        }

        public override byte ReadByte()
        {
            return _data[_offset++];
        }

        public override ArraySegment ReadBytes(uint length)
        {
            //uint i = 0;
            //byte[] arraySegment = new byte[length];
            //while(i < length)
            //{
            //    arraySegment[i] = _data[_offset++];
            //    i++;
            //}
            //return arraySegment;

            var segment = new ArraySegment(_data, _offset, length);
            _offset += length;
            return segment;
        }

        public override void Seek(long offset, SeekOrigin origin)
        {
            _offset = origin switch
            {
                SeekOrigin.Begin => (uint)offset,
                SeekOrigin.Current => (uint)(_offset + offset),
                SeekOrigin.End => (uint)(_data.Length + offset),
                _ => throw new ArgumentOutOfRangeException(nameof(origin), origin.ToString()),
            };
        }
#nullable enable
        protected override ArraySegment? StopTokenGathering()
        {
            if ((_firstGatheredByte + 1) <= _data.Length)
                return new ArraySegment(_data, (int)_firstGatheredByte, (int)(_offset - _firstGatheredByte));
            else
                return null;
        }

        protected override void StartTokenGathering()
        {
            _firstGatheredByte = _offset;
        }
    }
}
