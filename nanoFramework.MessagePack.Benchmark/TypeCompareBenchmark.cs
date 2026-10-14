// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using nanoFramework.MessagePack.Benchmark.Base;

namespace nanoFramework.MessagePack.Benchmark
{
    /// <summary>
    /// Comparative Type compare benchmark.
    /// </summary>
    [IterationCount(5)]
    public class TypeCompareBenchmark : BaseIterationBenchmark
    {
        private const int COMPARE_COUNT = 100;
        private readonly Type _intType = typeof(int);
        private readonly int _intTypeHash = typeof(int).FullName.GetHashCode();


        /// <summary>
        /// Type FullName hash comparer benchmark.
        /// </summary>
        [Benchmark]
        public void TestTypeCompareByNameBenchmark()
        {
            RunInIteration(() =>
            {
                int count = COMPARE_COUNT;
                while (count-- > 0)
                {
                    _ = _intTypeHash == _intType.FullName.GetHashCode();
                }
            });
        }

        /// <summary>
        /// Type instance comparer benchmark.
        /// </summary>
        [Benchmark]
        public void TestTypeCompareByTypeBenchmark()
        {
            RunInIteration(() =>
            {
                int count = COMPARE_COUNT;
                while (count-- > 0)
                {
                    _ = _intType == typeof(int);
                }
            });
        }
    }
}
