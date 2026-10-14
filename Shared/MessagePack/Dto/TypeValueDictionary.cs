// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Collections;

namespace nanoFramework.MessagePack.Dto
{
    /// <summary>
    /// Represents a dictionary class for storing <see cref="DictionaryEntry"/> <see cref="Type"/> and <see cref="object"/> values.
    /// </summary>
    public class TypeValueDictionary : ICollection
    {
        private readonly ArrayList _dictionaryEntries = new();

#nullable enable
        /// <summary>
        /// Sets or Gets the element at the given key.
        /// </summary>
        /// <param name="key">Key <see cref="Type"/> value.</param>
        /// <returns><see cref="object"/> value or <see langword="null"/>.</returns>
        public object? this[Type key]
        {
            get
            {
#if NANOFRAMEWORK_1_0
                return GetElement(key)?.Value;
#else
                return GetElement(key, out _)?.Value;
#endif
            }

            set
            {
                lock (_dictionaryEntries.SyncRoot)
                {
#if NANOFRAMEWORK_1_0
                    DictionaryEntry? element = GetElement(key);
#else
                    DictionaryEntry? element = GetElement(key, out int index);
#endif
                    if (element != null)
                    {
#if NANOFRAMEWORK_1_0
                        element.Value = value;
#else
                        _dictionaryEntries[index] = new DictionaryEntry(key, value);
#endif
                    }
                    else
                    {
                        _dictionaryEntries.Add(new DictionaryEntry(key, value));
                    }
                }
            }
        }

        int ICollection.Count => _dictionaryEntries.Count;

        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot => _dictionaryEntries.SyncRoot;

        private static DictionaryEntry ToDictionaryEntry(object? obj)
        {
            if (obj == null)
            {
#if NANOFRAMEWORK_1_0
                throw new ArgumentNullException();
#else
                throw new ArgumentNullException(nameof(obj));
#endif
            }
#if NANOFRAMEWORK_1_0
            return obj as DictionaryEntry ?? throw new InvalidCastException();
#else
            return (DictionaryEntry)obj;
#endif
        }

        void ICollection.CopyTo(Array array, int index) => _dictionaryEntries.CopyTo(array, index);

        IEnumerator IEnumerable.GetEnumerator() => _dictionaryEntries.GetEnumerator();

        /// <summary>
        /// Add new <see cref="DictionaryEntry"/> in to dictionary.
        /// </summary>
        /// <param name="key">Key <see cref="Type"/> value.</param>
        /// <param name="value">Value <see cref="object"/> instance.</param>
        /// <exception cref="ArgumentNullException">Value argument is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The <see cref="object"/> for a specific key <see cref="Type"/> is already in the dictionary.</exception>
        public void Add(Type key, object value)
        {
#if !NANOFRAMEWORK_1_0
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
#else
            if (value == null)
            {
                throw new ArgumentNullException();
            }
#endif
            lock (_dictionaryEntries.SyncRoot)
            {
#if NANOFRAMEWORK_1_0
                DictionaryEntry? element = GetElement(key);
#else
                DictionaryEntry? element = GetElement(key, out int index);
#endif
                if (element?.Value != null)
                {
#if !NANOFRAMEWORK_1_0
                    throw new ArgumentException(null, nameof(key));
#else
                    throw new ArgumentException();
#endif
                }
                else
                {
                    if (element == null)
                    {
                        _dictionaryEntries.Add(new DictionaryEntry(key, value));
                    }
                    else
                    {
#if NANOFRAMEWORK_1_0
                        element.Value = value;
#else
                        _dictionaryEntries[index] = new DictionaryEntry(key, value);
#endif
                    }
                }
            }
        }

        /// <summary>
        /// Remove <see cref="object"/> from a dictionary by key <see cref="Type"/>.
        /// </summary>
        /// <param name="key">Key <see cref="Type"/> value.</param>
        public void Remove(Type key)
        {
            lock (_dictionaryEntries.SyncRoot)
            {
                //// We do not delete the record to avoid breaking the search loop in multi-threaded access
#if NANOFRAMEWORK_1_0
                DictionaryEntry? element = GetElement(key);
#else
                DictionaryEntry? element = GetElement(key, out int index);
#endif
                if (element != null)
                {
#if NANOFRAMEWORK_1_0
                    element.Value = null;
#else
                    _dictionaryEntries[index] = new DictionaryEntry(key, null);
#endif
                }
            }
        }
#if NANOFRAMEWORK_1_0
        private DictionaryEntry? GetElement(Type key)
        {
            //// We don't use foreach to avoid unnecessary memory allocations
            for (int i = 0; i < _dictionaryEntries.Count; i++)
            {
                DictionaryEntry entry = ToDictionaryEntry(_dictionaryEntries[i]);
                if (entry.Key is Type keyType && keyType == key)
                {
                    return entry;
                }
            }

            return null;
        }
#else
        private DictionaryEntry? GetElement(Type key, out int index)
        {
            //// We don't use foreach to avoid unnecessary memory allocations
            for (int i = 0; i < _dictionaryEntries.Count; i++)
            {
                DictionaryEntry entry = ToDictionaryEntry(_dictionaryEntries[i]);
                if (entry.Key is Type keyType && keyType == key)
                {
                    index = i;
                    return entry;
                }
            }

            index = -1;
            return null;
        }
#endif
    }
}
