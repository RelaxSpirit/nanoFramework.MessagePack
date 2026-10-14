// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization.Formatters.Binary;
using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using nanoFramework.Json;
using nanoFramework.MessagePack.Benchmark.Base;
using nanoFramework.MessagePack.Benchmark.Data;

namespace nanoFramework.MessagePack.Benchmark.SerializationBenchmarks
{
    /// <summary>
    /// Comparative serialization benchmark.
    /// </summary>
    [IterationCount(5)]
    public class ComparativeSerializationBenchmark : BaseIterationBenchmark
    {
        /// <summary>
        /// Serialization json text <see cref="ComparativeTestObjects.TestObject"/> benchmark.
        /// </summary>
        [Benchmark]
        public void JsonSerializationBenchmark()
        {
            RunInIteration(() =>
            {
                _ = JsonConvert.SerializeObject(ComparativeTestObjects.TestObject);
            });
        }

        /// <summary>
        /// Serialization <see cref="MessagePack"/> test <see cref="ComparativeTestObjects.TestObject"/> benchmark.
        /// </summary>
        [Benchmark]
        public void MessagePackSerializationBenchmark()
        {
            RunInIteration(() =>
            {
                _ = MessagePackSerializer.Serialize(ComparativeTestObjects.TestObject);
            });
        }

        /// <summary>
        /// Serialization <see cref="BinaryFormatter"/> test <see cref="ComparativeTestObjects.TestObject"/> benchmark.
        /// </summary>
        [Benchmark]
        public void BinarySerializationBenchmark()
        {
            RunInIteration(() =>
            {
                _ = BinaryFormatter.Serialize(ComparativeTestObjects.TestObject);
            });
        }
    }
}
