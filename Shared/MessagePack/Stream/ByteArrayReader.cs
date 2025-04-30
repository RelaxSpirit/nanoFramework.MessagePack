// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using nanoFramework.MessagePack.Dto;

namespace nanoFramework.MessagePack.Stream
{
    internal class ByteArrayReader : BaseReader
    {
        private readonly byte[] _data;

        private uint _firstGatheredByte;
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
            var segment = new ArraySegment(_data, _offset, length);
            _offset += length;
            return segment;
        }

        public override void Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    _offset = (uint)offset;
                    break;
                case SeekOrigin.Current:
                    _offset = (uint)(_offset + offset);
                    break;
                case SeekOrigin.End:
                    _offset = (uint)(_data.Length + offset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
#nullable enable
        protected override ArraySegment? StopTokenGathering()
        {
            if (_firstGatheredByte <= _data.Length)
            {
                return new ArraySegment(_data, (int)_firstGatheredByte, (int)(_offset - _firstGatheredByte));
            }
            else
            {
                return null;
            }
        }

        protected override void StartTokenGathering()
        {
            _firstGatheredByte = _offset;
        }
    }
}
