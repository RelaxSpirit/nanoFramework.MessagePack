// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
using System.Collections;
using System.Reflection;

namespace nanoFramework.MessagePack.Dto
{
    internal class MemberPropertyInfo : PropertyInfo
    {
        internal const string GET_ = "get_";
        internal const string SET_ = "set_";
#nullable enable
        private readonly MethodInfo? _getMethodInfo;
        private readonly MethodInfo? _setMethodInfo;

        private MemberPropertyInfo(MethodInfo? getMethod, MethodInfo? setMethod)
        {
            MethodInfo? mi;

            if (getMethod != null)
            {
                PropertyType = getMethod.ReturnType;
                mi = getMethod;
            }
            else if (setMethod != null)
            {
                ParameterInfo[] parametersInfo = setMethod.GetParameters();
                if (parametersInfo.Length == 1)
                {
                    PropertyType = parametersInfo[0].ParameterType;
                    mi = setMethod;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                throw new ArgumentNullException();
            }

            _getMethodInfo = getMethod;
            _setMethodInfo = setMethod;

            DeclaringType = mi.DeclaringType;
            if (mi.Name.Length > 4)
            {
                Name = mi.Name.Substring(4);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public override Type PropertyType { get; }

        public override MemberTypes MemberType => MemberTypes.Property;

        public override string Name { get; }

        public override Type DeclaringType { get; }

        private static Hashtable GetPropertiesMethods(Type type, BindingFlags bindingAttr)
        {
            var propertyHashtable = new Hashtable();
            MethodInfo[] methods = type.GetMethods(bindingAttr);

            foreach (MethodInfo method in methods)
            {
                if (ShouldSerializeMethod(method))
                {
                    string propertyName = method.Name.Substring(4);
                    var methodInfos = (MethodInfo?[])propertyHashtable[propertyName];
                    if (methodInfos == null)
                    {
                        methodInfos = new MethodInfo?[2];
                        propertyHashtable[propertyName] = methodInfos;
                    }

                    methodInfos[method.Name.StartsWith(GET_) ? 0 : 1] = method;
                }
            }
            return propertyHashtable;
        }

        private static bool ShouldSerializeMethod(MethodInfo method)
        {
            // We care only about property getters when serializing and setter deserializing
            if (!method.Name.StartsWith(GET_) && !method.Name.StartsWith(SET_))
            {
                return false;
            }

            // Ignore abstract and virtual objects
            if (method.IsAbstract)
            {
                return false;
            }

            // Ignore static methods
            if (method.IsStatic)
            {
                return false;
            }

            // Ignore delegates and MethodInfos
            if ((method.ReturnType == typeof(Delegate)) ||
                (method.ReturnType == typeof(MulticastDelegate)) ||
                (method.ReturnType == typeof(MethodInfo)))
            {
                return false;
            }

            // Ditto for DeclaringType
            if ((method.DeclaringType == typeof(Delegate)) ||
                (method.DeclaringType == typeof(MulticastDelegate)))
            {
                return false;
            }

            return true;
        }

        public static MemberPropertyInfo[] GetProperties(Type type, BindingFlags bindingAttr)
        {
            var propertyHashtable = GetPropertiesMethods(type, bindingAttr);

            MemberPropertyInfo[] result = new MemberPropertyInfo[propertyHashtable.Count];

            int index = 0;
            foreach (DictionaryEntry entry in propertyHashtable)
            {
                if (entry.Value is MethodInfo?[] methodInfos)
                {
                    result[index] = new MemberPropertyInfo(methodInfos[0], methodInfos[1]);
                    index++;
                }
                else
                {
                    throw new ArgumentException();
                }
            }

            return result;
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            object[] getAttrs = _getMethodInfo?.GetCustomAttributes(inherit) ?? new object[0];
            object[] setAttrs = _setMethodInfo?.GetCustomAttributes(inherit) ?? new object[0];

            object[] resultAttrs = new object[getAttrs.Length + setAttrs.Length];
            Array.Copy(getAttrs, 0, resultAttrs, 0, getAttrs.Length);
            Array.Copy(setAttrs, 0, resultAttrs, getAttrs.Length, setAttrs.Length);
            return resultAttrs;
        }

        public override object? GetValue(object obj, object[] parameters)
        {
            return _getMethodInfo?.Invoke(obj, null);
        }

        public override void SetValue(object obj, object? value, object[] index)
        {
            _setMethodInfo?.Invoke(obj, new object?[] { value });
        }
    }
}
#endif
