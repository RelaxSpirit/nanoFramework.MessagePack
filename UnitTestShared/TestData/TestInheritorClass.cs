// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace UnitTestShared.TestData
{
    internal class TestInheritorClass : TestBaseClass
    {
        public string SimpleField = string.Empty;

        public override string Name { get; set; } = string.Empty;

        public override int VirtualProperty { get; set; } = -100;
    }
}
