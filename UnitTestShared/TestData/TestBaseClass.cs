// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace UnitTestShared.TestData
{
    public abstract class TestBaseClass
    {
        public string BaseField = "baseField";

        public abstract string Name { get; set; }

        public virtual int VirtualProperty { get; set; } = 100;

        public bool SimpleProperty { get; set; } = false;
    }
}
