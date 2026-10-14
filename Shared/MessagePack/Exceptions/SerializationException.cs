// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace nanoFramework.MessagePack.Exceptions
{
    /// <summary>
    /// Exception for serialization errors.
    /// </summary>
    public class SerializationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationException" /> class.
        /// </summary>
        /// <param name="message">Error message.</param>
        public SerializationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationException" /> class.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="ex">Inner exception.</param>
        public SerializationException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
