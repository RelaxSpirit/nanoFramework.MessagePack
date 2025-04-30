// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Collections;
using nanoFramework.MessagePack.Dto;
using nanoFramework.MessagePack.Utility;

namespace nanoFramework.MessagePack.Stream
{
    internal sealed class MemoryStreamReader : BaseReader, IDisposable
    {
        private readonly MemoryStream _stream;
        private readonly ArrayList _bytesGatheringBuffer;

        private long _bytesGatheringBufferLength = 0;
        private bool _bytesGatheringInProgress;

        public MemoryStreamReader(MemoryStream stream)
        {
            _stream = stream;
            _bytesGatheringBuffer = new ArrayList();
        }

        public override byte ReadByte()
        {
            var temp = _stream.ReadByte();
            if (temp == -1)
            {
                throw ExceptionUtility.NotEnoughBytes(0, 1);
            }

            var result = (byte)temp;
            if (_bytesGatheringInProgress)
            {
                _bytesGatheringBuffer.Add(new byte[] { result });
                _bytesGatheringBufferLength++;
            }

            return result;
        }

        public override ArraySegment ReadBytes(uint length)
        {
            var buffer = ReadBytesInternal(length);

            if (_bytesGatheringInProgress)
            {
                _bytesGatheringBuffer.Add(buffer);
                _bytesGatheringBufferLength += buffer.Length;
            }

            return buffer;
        }

        public override void Seek(long offset, SeekOrigin origin)
        {
            if (_bytesGatheringInProgress)
            {
                var buffer = ReadBytesInternal((uint)offset);

                _bytesGatheringBuffer.Add(buffer);
                _bytesGatheringBufferLength += buffer.Length;
            }
            else
            {
                _stream.Seek(offset, origin);
            }
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        private byte[] ReadBytesInternal(uint length)
        {
            var buffer = new byte[length];
            var read = _stream.Read(buffer, 0, buffer.Length);
            if (read < buffer.Length)
            {
                throw ExceptionUtility.NotEnoughBytes(read, buffer.Length);
            }

            return buffer;
        }
#nullable enable
        protected override ArraySegment? StopTokenGathering()
        {
            if (_stream.Position <= _stream.Length)
            {
                _bytesGatheringInProgress = false;

                byte[] result = new byte[_bytesGatheringBufferLength];

                int destinationOffset = 0;
                foreach (byte[] part in _bytesGatheringBuffer)
                {
                    Array.Copy(part, 0, result, destinationOffset, part.Length);
                    destinationOffset += part.Length;
                }

                return new ArraySegment(result, 0, result.Length);
            }
            else
            {
                return null;
            }
        }

        protected override void StartTokenGathering()
        {
            _bytesGatheringInProgress = true;
            _bytesGatheringBuffer.Clear();
            _bytesGatheringBufferLength = 0;
        }
    }
}
