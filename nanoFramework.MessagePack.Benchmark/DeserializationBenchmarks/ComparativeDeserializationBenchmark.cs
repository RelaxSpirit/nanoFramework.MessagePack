// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization.Formatters.Binary;
using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using nanoFramework.Json;
using nanoFramework.MessagePack.Benchmark.Base;
using nanoFramework.MessagePack.Benchmark.Data;
using static nanoFramework.MessagePack.Benchmark.Data.ComparativeTestObjects;

namespace nanoFramework.MessagePack.Benchmark.DeserializationBenchmarks
{
    /// <summary>
    /// Comparative deserialization benchmark.
    /// </summary>
    [IterationCount(5)]
    public class ComparativeDeserializationBenchmark : BaseIterationBenchmark
    {
        /// <summary>
        /// Deserialization json benchmark.
        /// </summary>
        [Benchmark]
        public void JsonDeserializationBenchmark()
        {
            RunInIteration(() =>
            {
                _ = JsonConvert.DeserializeObject(TestJson, typeof(ComparativeBenchmarkObject), ComparativeTestObjects.JsonSerializerOptions);
            });
        }

        /// <summary>
        /// Deserialization <see cref="MessagePack"/> binary array benchmark.
        /// </summary>
        [Benchmark]
        public void MessagePackDeserializationBenchmark()
        {
            RunInIteration(() =>
            {
                _ = MessagePackSerializer.Deserialize(typeof(ComparativeBenchmarkObject), TestObjectMsgPackBytes);
            });
        }

        /// <summary>
        /// Deserialization <see cref="BinaryFormatter"/> binary array benchmark.
        /// </summary>
        [Benchmark]
        public void BinaryDeserializationBenchmark()
        {
            RunInIteration(() =>
            {
                _ = BinaryFormatter.Deserialize(TestObjectBinaryBytes);
            });
        }
    }
}
