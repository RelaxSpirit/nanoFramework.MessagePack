#if !NANOFRAMEWORK_1_0
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using nanoFramework.MessagePack.Stream;

namespace nanoFramework.MessagePack.Converters
{
    /// <summary>
    /// The enum from string converter class.
    /// </summary>
    /// <typeparam name="TEnum">Enum type.</typeparam>
    public class FromStringEnumConverter<TEnum> : FromStringEnumConverter where TEnum : struct, IConvertible
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FromStringEnumConverter" /> class.
        /// </summary>
        public FromStringEnumConverter() : base(typeof(TEnum).GetTypeInfo())
        {
        }

#nullable enable
        public override object? Read([NotNull] IMessagePackReader reader)
        {
            return (TEnum) base.Read(reader)!;
        }

        public override void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            base.Write((TEnum)value!, writer);
        }
    }

    /// <summary>
    /// The enum from string converter class.
    /// </summary>
    public class FromStringEnumConverter : IConverter
    {
        private readonly Type _enumType;

        internal FromStringEnumConverter(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException($"Parameter value type is {enumType} and it is not Enum type.");
            }

            _enumType = enumType;
        }

        public virtual object? Read([NotNull] IMessagePackReader reader)
        {
            var enumString = (string)ConverterContext.GetConverter(typeof(string)).Read(reader)!;

            return Enum.Parse(_enumType, enumString, true);
        }

        public virtual void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            if (value == null)
            {
                ConverterContext.NullConverter.Write(value, writer);
            }
            else
            {
                var valueType = value.GetType();

                if (!value.GetType().IsEnum)
                    throw new ArgumentException($"Value is type {valueType} and it is not Enum type.", nameof(value));

                if(valueType.FullName != _enumType.FullName)
                    throw new ArgumentException($"Value is type {valueType} is not equal current converter type {_enumType}.", nameof(value));

                ConverterContext.GetConverter(typeof(string)).Write(value.ToString(), writer);
            }
        }
    }
}
#endif
