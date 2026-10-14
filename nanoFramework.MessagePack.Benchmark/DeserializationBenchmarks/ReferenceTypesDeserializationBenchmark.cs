// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using nanoFramework.MessagePack.Benchmark.Base;
using nanoFramework.MessagePack.Benchmark.Data;

namespace nanoFramework.MessagePack.Benchmark.DeserializationBenchmarks
{
    /// <summary>
    /// Reference types deserialization benchmark.
    /// </summary>
    [IterationCount(5)]
    public class ReferenceTypesDeserializationBenchmark : BaseIterationBenchmark
    {
        /// <summary>
        /// Bates int array for test.
        /// </summary>
        private byte[] intArrayBytes;

        /// <summary>
        /// Byte array for test.
        /// </summary>
        private byte[] byteArrayBytes;

        /// <summary>
        /// Bytes two-dimensional array for test.
        /// </summary>
        private byte[] twoDimensionalArrayBytes;

        /// <summary>
        /// Bytes hashtable array for test.
        /// </summary>
        private byte[] testHashtableBytes;

        /// <summary>
        /// Initialize all test data objects.
        /// </summary>
        [Setup]
        public void Setup()
        {
            intArrayBytes = MessagePackSerializer.Serialize(ReferenceTestObjects.IntArray);
            byteArrayBytes = MessagePackSerializer.Serialize(ReferenceTestObjects.ByteArray);
            twoDimensionalArrayBytes = MessagePackSerializer.Serialize(ReferenceTestObjects.TwoDimensionalArray);
            testHashtableBytes = MessagePackSerializer.Serialize(ReferenceTestObjects.TestHashtable);
        }

        /// <summary>
        /// Deserialize int array benchmark
        /// </summary>
        [Benchmark]
        public void IntArrayBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(int[]), intArrayBytes);
            });
        }

        /// <summary>
        /// Deserialize byte array benchmark
        /// </summary>
        [Benchmark]
        public void ByteArrayBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(byte[]), byteArrayBytes);
            });
        }

        /// <summary>
        /// Deserialize two-dimensional array benchmark.
        /// </summary>
        [Benchmark]
        public void TwoDimensionalArrayBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(long[][]), twoDimensionalArrayBytes);
            });
        }

        /// <summary>
        /// /// <summary>
        /// Deserialize hashtable benchmark.
        /// </summary>
        /// </summary>
        [Benchmark]
        public void TestHashtableBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(Hashtable), testHashtableBytes);
            });
        }
    }
}
