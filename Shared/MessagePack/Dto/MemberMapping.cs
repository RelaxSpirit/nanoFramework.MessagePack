// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Reflection;

namespace nanoFramework.MessagePack.Dto
{
    internal class MemberMapping
    {
#nullable enable
        private readonly FieldInfo? _field;
        private readonly PropertyInfo? _property;
        
        public MemberMapping(PropertyInfo property)
        {
            _property = property;
            Name = property.Name;
            MemberType = property.PropertyType;        
        }

        public MemberMapping(FieldInfo field)
        {
            _field = field;
            Name = field.Name;
            MemberType = field.FieldType;
        }

        public string Name { get; }

        public Type MemberType { get; }

        public bool TryGetValue(object obj, out object? value)
        {
            if (_field != null)
            {
                value = _field.GetValue(obj);
                return true;
            }

            if (_property != null)
            {
                value = _property.GetValue(obj, null);
                return true;
            }

            value = null;
            return false;
        }
#if DEBUG
        public override string ToString()
        {
            return $"{_field?.Name ?? string.Empty}{_property?.Name ?? string.Empty}";
        }
#endif
        internal void SetValue(object current, object? val)
        {
            if (_field != null)
            {
                _field.SetValue(current, val);
            }
            else
            {
                _property?.SetValue(current, val, null);
            }
        }
    }
}
