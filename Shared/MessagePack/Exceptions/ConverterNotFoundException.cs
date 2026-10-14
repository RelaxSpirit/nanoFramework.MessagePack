// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif

namespace nanoFramework.MessagePack.Exceptions
{
    /// <summary>
    /// Exception for converter not found.
    /// </summary>
    public class ConverterNotFoundException : Exception
    {
        /// <summary>
        /// Gets the type of object which converter was not found.
        /// </summary>
        public Type ObjectType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterNotFoundException" /> class.
        /// </summary>
        /// <param name="type">Type of object.</param>
        public ConverterNotFoundException(Type type)
            : base($"Converter not found for type {type.FullName}")
        {
            ObjectType = type;
        }
    }
}
