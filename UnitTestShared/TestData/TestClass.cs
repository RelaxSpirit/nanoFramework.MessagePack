// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Collections;
using UnitTestShared.Helpers;

namespace UnitTestShared.TestData
{
    public class TestClass : IEqualityComparer
    {
        public sbyte fieldTest;
        public char[] CharArray { get; set; }

#if NANOFRAMEWORK_1_0
        public int Id { get; set; }
#else
        public int? Id { get; set; }
#endif
        public string Name { get; set; }

        public float[] FloatArray { get; set; }

        public long[][] LongTwoDimensionalArray { get; set; }

        public string[] StringArray { get; set; }

        public TestSubClass SubTestObject { get; set; }

        public TestSubClass[] SubTestObjectArray { get; set; }

        public ArrayList TestArrayList { get; set; } = new();

        public DateTime TestDateTimeNow { get; private set; } = DateTime.UtcNow;

        public TimeSpan TestTimeSpan { get; private set; } = DateTime.UtcNow.TimeOfDay - TimeSpan.FromHours(1);

#nullable enable
        public new bool Equals(object? x, object? y)
        {
            if (x == null && x == y)
            {
                return true;
            }

            return (x is TestClass x_) &&
                (y is TestClass y_) &&
                x_.TestDateTimeNow == y_.TestDateTimeNow &&
                x_.TestTimeSpan == y_.TestTimeSpan &&
                x_.Id == y_.Id &&
                x_.Name == y_.Name &&
                x_.FloatArray.ArrayEqual(y_.FloatArray) &&
                x_.StringArray.ArrayEqual(y_.StringArray) &&
                x_.LongTwoDimensionalArray.ArrayEqual(y_.LongTwoDimensionalArray) &&
                x_.SubTestObjectArray.ArrayEqual(y_.SubTestObjectArray) &&
                x_.TestArrayList.ArrayEqual(y_.TestArrayList) &&
                x_.CharArray.ArrayEqual(y_.CharArray) &&
                x_.fieldTest == y_.fieldTest;
        }

        public override bool Equals(object? obj)
        {
            return Equals(this, obj);
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }
        public int GetHashCode(object obj)
        {
            return 0;
        }
    }
}
