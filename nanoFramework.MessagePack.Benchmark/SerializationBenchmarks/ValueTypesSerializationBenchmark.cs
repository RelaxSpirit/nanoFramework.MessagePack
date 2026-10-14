// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using nanoFramework.MessagePack.Benchmark.Base;

namespace nanoFramework.MessagePack.Benchmark.SerializationBenchmarks
{
    /// <summary>
    /// Value types serialization benchmark
    /// </summary>
    [IterationCount(5)]
    public class ValueTypesSerializationBenchmark : BaseIterationBenchmark
    {
        /// <summary>
        /// Test <see cref="Guid"/>
        /// </summary>
        private readonly Guid _testGuid = Guid.NewGuid();

        /// <summary>
        /// Test <see cref="DateTime"/>
        /// </summary>
        private readonly DateTime _testDateTime = DateTime.UtcNow;

        /// <summary>
        /// Test <see cref="string"/>
        /// </summary>
        private readonly string _testString = "Benchmark test";

        /// <summary>
        /// Serialization <see cref="Guid"/> benchmark.
        /// </summary>
        [Benchmark]
        public void GuidBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Serialize(_testGuid);
            });
        }

        /// <summary>
        /// Serialization <see cref="short"/> benchmark.
        /// </summary>
        [Benchmark]
        public void ShortBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Serialize(short.MaxValue);
            });
        }

        /// <summary>
        /// Serialization <see cref="ushort"/> benchmark.
        /// </summary>
        [Benchmark]
        public void UshortBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Serialize(ushort.MaxValue);
            });
        }

        /// <summary>
        /// Serialization <see cref="int"/> benchmark.
        /// </summary>
        [Benchmark]
        public void IntBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Serialize(int.MaxValue);
            });
        }

        /// <summary>
        /// Serialization <see cref="uint"/> benchmark.
        /// </summary>
        [Benchmark]
        public void UintBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Serialize(uint.MaxValue);
            });
        }

        /// <summary>
        /// Serialization <see cref="sbyte"/> benchmark.
        /// </summary>
        [Benchmark]
        public void SbyteBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Serialize(sbyte.MaxValue);
            });
        }

        /// <summary>
        /// Serialization <see cref="byte"/> benchmark.
        /// </summary>
        [Benchmark]
        public void ByteBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Serialize(byte.MaxValue);
            });
        }

        /// <summary>
        /// Serialization <see cref="bool"/> benchmark.
        /// </summary>
        [Benchmark]
        public void BoolBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Serialize(true);
            });
        }

        /// <summary>
        /// Serialization <see cref="char"/> benchmark.
        /// </summary>
        [Benchmark]
        public void CharBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Serialize(char.MaxValue);
            });
        }

        /// <summary>
        /// Serialization <see cref="string"/> benchmark.
        /// </summary>
        [Benchmark]
        public void StringBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Serialize(_testString);
            });
        }

        /// <summary>
        /// Serialization <see cref="TimeSpan"/> benchmark.
        /// </summary>
        [Benchmark]
        public void TimeSpanBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Serialize(TimeSpan.MaxValue);
            });
        }

        /// <summary>
        /// Serialization <see cref="float"/> benchmark.
        /// </summary>
        [Benchmark]
        public void FloatBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Serialize(float.MaxValue);
            });
        }

        /// <summary>
        /// Serialization <see cref="double"/> benchmark.
        /// </summary>
        [Benchmark]
        public void DoubleBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Serialize(double.MaxValue);
            });
        }

        /// <summary>
        /// Serialization <see cref="DateTime"/> benchmark.
        /// </summary>
        [Benchmark]
        public void DateTimeBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Serialize(_testDateTime);
            });
        }

        /// <summary>
        /// Serialization <see cref="long"/> benchmark.
        /// </summary>
        [Benchmark]
        public void LongBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Serialize(long.MaxValue);
            });
        }

        /// <summary>
        /// Serialization <see cref="ulong"/> benchmark.
        /// </summary>
        [Benchmark]
        public void UlongBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Serialize(ulong.MaxValue);
            });
        }
    }
}
