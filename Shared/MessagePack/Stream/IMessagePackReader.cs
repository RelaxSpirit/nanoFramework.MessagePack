// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using nanoFramework.MessagePack.Dto;

namespace nanoFramework.MessagePack.Stream
{
    /// <summary>
    /// Read <see cref="MessagePack"/> interface.
    /// </summary>
    public interface IMessagePackReader
    {
        /// <summary>
        /// Read MessagePack data type.
        /// </summary>
        /// <returns>The data type read.</returns>
        DataTypes ReadDataType();

        /// <summary>
        /// Read one byte in <see cref="MessagePack"/> stream.
        /// </summary>
        /// <returns>The byte read.</returns>
        byte ReadByte();

        /// <summary>
        /// Read bytes in <see cref="MessagePack"/> stream.
        /// </summary>
        /// <param name="length">Length of bytes to read.</param>
        /// <returns>The read bytes as <see cref="ArraySegment"/>.</returns>
        ArraySegment ReadBytes(uint length);

        /// <summary>
        /// Seek bytes in MessagePack stream.
        /// </summary>
        /// <param name="offset">Offset from stream.</param>
        /// <param name="origin">The position in the flow to offset.</param>
        void Seek(long offset, SeekOrigin origin);

        /// <summary>
        /// Read MessagePack array length.
        /// </summary>
        /// <returns>The length of the array.</returns>
        uint ReadArrayLength();

        /// <summary>
        /// Read <see cref="MessagePack"/> map length.
        /// </summary>
        /// <returns>The length of the map.</returns>
        uint ReadMapLength();

        /// <summary>
        /// Skip <see cref="MessagePack"/> item.
        /// </summary>
        void SkipToken();

        /// <summary>
        /// Read <see cref="MessagePack"/> item.
        /// </summary>
        /// <returns> The read <see cref="MessagePack"/> item as <see cref="ArraySegment"/> or <see langword="null"/> if the end of the stream is reached.</returns>
#nullable enable
        ArraySegment? ReadToken();
    }
}
