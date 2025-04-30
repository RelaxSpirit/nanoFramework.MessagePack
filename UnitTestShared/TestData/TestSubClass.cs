// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Collections;
using UnitTestShared.Helpers;

namespace UnitTestShared.TestData
{
    public class TestSubClass : IEqualityComparer
    {
#if !NANOFRAMEWORK_1_0
        public DateTime? NullableDateTime { get; set; }
#endif
        public Hashtable TestHashtable { get; set; }

        public byte[] Bytes { get; set; }

        public int[] Ints { get; set; }

        public double[] DoubleArray { get; set; }

        public Guid[] GuidArray { get; set; }
#nullable enable
        public new bool Equals(object? x, object? y)
        {
            if (x == null && x == y)
            {
                return true;
            }

            return (x is TestSubClass x_) &&
                (y is TestSubClass y_) &&
                x_.Bytes.ArrayEqual(y_.Bytes) &&
                x_.Ints.ArrayEqual(y_.Ints) &&
                x_.DoubleArray.ArrayEqual(y_.DoubleArray) &&
                x_.GuidArray.ArrayEqual(y_.GuidArray) &&
                x_.TestHashtable.DictionaryEqual(y_.TestHashtable)
#if !NANOFRAMEWORK_1_0
                && x_.NullableDateTime == y_.NullableDateTime;
#else
                ;
#endif
        }

        public int GetHashCode(object obj)
        {
            return 0;
        }

        public override bool Equals(object? obj)
        {
            return Equals(this, obj);
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }
    }
}
