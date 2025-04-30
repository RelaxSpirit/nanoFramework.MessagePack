// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Collections;
using nanoFramework.MessagePack.Stream;
using nanoFramework.MessagePack.Utility;

namespace nanoFramework.MessagePack.Dto
{
    /// <summary>
    /// Segment by byte to array.
    /// </summary>
    public class ArraySegment : BaseReader, IEnumerable
    {
        private readonly byte[] _buffer;
        private readonly long _offset;
        private readonly long _length;

        private int _firstGatheredByte;

        /// <summary>
        /// Gets byte by array segment.
        /// </summary>
        /// <param name="index">The byte index in the array segment.</param>
        /// <returns>Byte value.</returns>
        public byte this[int index]
        {
            get
            {
                return _buffer[_offset + index];
            }
        }

        internal byte[] SourceBuffer => _buffer;

        internal long SourceOffset => _offset;

        internal long Length => _length;

        /// <summary>
        /// Gets the current position in the segment.
        /// </summary>
        public int Position { get; private set; } = 0;

        /// <summary>
        /// Implicit conversion from byte array to <see cref="ArraySegment"/>.
        /// </summary>
        /// <param name="bytes">Source byte array.</param>
        public static implicit operator ArraySegment(byte[] bytes)
        {
            return new ArraySegment(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Implicit conversion from <see cref="ArraySegment"/> to byte array.
        /// </summary>
        /// <param name="segment">Source <see cref="ArraySegment"/>.</param>
        public static explicit operator byte[](ArraySegment segment)
        {
            return segment.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArraySegment" /> class.
        /// </summary>
        /// <param name="buffer">Source byte array.</param>
        /// <param name="offset">Offset in source byte array.</param>
        /// <param name="length">Length segment.</param>
        public ArraySegment(byte[] buffer, long offset, long length)
        {
            _buffer = buffer;
            _offset = offset;
            _length = length;
        }

        /// <summary>
        /// Reads bytes in an array segment.
        /// </summary>
        /// <param name="length">Required reading length.</param>
        /// <returns>Segment by byte to current segment.</returns>
        public override ArraySegment ReadBytes(uint length)
        {
            var segment = new ArraySegment(_buffer, _offset + Position, length);
            Position += (int)length;
            return segment;
        }

        /// <summary>
        /// Move the reading position in the array segment.
        /// </summary>
        /// <param name="offset">Offset in bytes.</param>
        /// <param name="origin">Offset reference point.</param>
        /// <exception cref="ArgumentOutOfRangeException">Unknown <see cref="SeekOrigin"/> value.</exception>
        public override void Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = (int)offset;
                    break;
                case SeekOrigin.Current:
                    Position = (int)(Position + offset);
                    break;
                case SeekOrigin.End:
                    Position = (int)(_length + offset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Read one byte from the segment.
        /// </summary>
        /// <returns>The byte read.</returns>
        public override byte ReadByte()
        {
            if (Position >= _length)
            {
                throw ExceptionUtility.NotEnoughBytes(Position, _length);
            }

            if (_offset + Position >= _buffer.Length)
            {
                throw ExceptionUtility.NotEnoughBytes(_offset + Position, _buffer.Length);
            }

            return _buffer[_offset + Position++];
        }

        /// <summary>
        /// Get bytes enumerator.
        /// </summary>
        /// <returns>Enumerator for bytes in segment.</returns>
        public IEnumerator GetEnumerator()
        {
            return new ArraySegmentEnumerator(this);
        }

        private byte[] ToArray()
        {
            var data = new byte[_length];

            System.Array.Copy(_buffer, (int)_offset, data, 0, (int)_length);
            return data;
        }

        /// <summary>
        /// Stopping the collection of MessagePack token in <see cref="ArraySegment"/>.
        /// </summary>
        /// <returns>Array segment bytes <see cref="ArraySegment"/>.</returns>
#nullable enable
        protected override ArraySegment? StopTokenGathering()
        {
            if (_firstGatheredByte <= _length)
            {
                var result = new ArraySegment(_buffer, (int)_firstGatheredByte + _offset, (int)(Position - _firstGatheredByte));
                _firstGatheredByte = 0;
                return result;
            }
            else
            {
                _firstGatheredByte = 0;
                return null;
            }
        }

        /// <summary>
        /// Start the collection of MessagePack token in <see cref="ArraySegment"/>.
        /// </summary>
        protected override void StartTokenGathering()
        {
            _firstGatheredByte = Position;
        }

        /// <summary>
        /// Enumerator in an array segment.
        /// </summary>
        public class ArraySegmentEnumerator : IEnumerator
        {
            private int _position = -1;
            private ArraySegment _arraySegment;

            internal ArraySegmentEnumerator(ArraySegment arraySegment)
            {
                _arraySegment = arraySegment;
            }

            /// <summary>
            /// Gets the element corresponding to the current position in the segment.
            /// </summary>
            public object Current
            {
                get
                {
                    if (_position >= _arraySegment._length)
                    {
                        throw ExceptionUtility.NotEnoughBytes(_position, _arraySegment._length);
                    }

                    try
                    {
                        return _arraySegment._buffer[_arraySegment._offset + _position];
                    }
                    catch
                    {
                        throw ExceptionUtility.NotEnoughBytes(_arraySegment._offset + _position, _arraySegment._buffer.Length);
                    }
                }
            }

            /// <summary>
            /// Go to next byte in array.
            /// </summary>
            /// <returns><see langword="true"/> if the end of the segment is not reached otherwise <see langword="false"/>.</returns>
            public bool MoveNext()
            {
                _position++;

                return _position < _arraySegment._length && _position + _arraySegment._offset < _arraySegment._buffer.Length;
            }

            /// <summary>
            /// Resets the current position to the beginning of the segment.
            /// </summary>
            public void Reset()
            {
                _position = -1;
            }
        }
    }
}
