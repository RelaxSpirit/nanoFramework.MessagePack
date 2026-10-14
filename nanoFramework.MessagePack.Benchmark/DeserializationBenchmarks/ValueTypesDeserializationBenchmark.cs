// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using nanoFramework.MessagePack.Benchmark.Base;

namespace nanoFramework.MessagePack.Benchmark.DeserializationBenchmarks
{
    /// <summary>
    /// Value types deserialization benchmark.
    /// </summary>
    [IterationCount(5)]
    public class ValueTypesDeserializationBenchmark : BaseIterationBenchmark
    {
        /// <summary>
        /// Test bytes for <see cref="short"/>
        /// </summary>
        private readonly byte[] shortBytes = MessagePackSerializer.Serialize(short.MaxValue);

        /// <summary>
        /// Test bytes for <see cref="ushort"/>
        /// </summary>
        private readonly byte[] ushortBytes = MessagePackSerializer.Serialize(ushort.MaxValue);

        /// <summary>
        /// Test bytes for <see cref="int"/>
        /// </summary>
        private readonly byte[] intBytes = MessagePackSerializer.Serialize(int.MaxValue);

        /// <summary>
        /// Test bytes for <see cref="uint"/>
        /// </summary>
        private readonly byte[] uintBytes = MessagePackSerializer.Serialize(uint.MaxValue);

        /// <summary>
        /// Test bytes for <see cref="long"/>
        /// </summary>
        private readonly byte[] longBytes = MessagePackSerializer.Serialize(long.MaxValue);

        /// <summary>
        /// Test bytes for <see cref="ulong"/>
        /// </summary>
        private readonly byte[] ulongBytes = MessagePackSerializer.Serialize(ulong.MaxValue);

        /// <summary>
        /// Test bytes for <see cref="byte"/>
        /// </summary>
        private readonly byte[] byteBytes = MessagePackSerializer.Serialize(byte.MaxValue);

        /// <summary>
        /// Test bytes for <see cref="sbyte"/>
        /// </summary>
        private readonly byte[] sbyteBytes = MessagePackSerializer.Serialize(sbyte.MaxValue);

        /// <summary>
        /// Test bytes for <see cref="float"/>
        /// </summary>
        private readonly byte[] floatBytes = MessagePackSerializer.Serialize(float.MaxValue);

        /// <summary>
        /// Test bytes for <see cref="double"/>
        /// </summary>
        private readonly byte[] doubleBytes = MessagePackSerializer.Serialize(double.MaxValue);

        /// <summary>
        /// Test bytes for <see cref="bool"/>
        /// </summary>
        private readonly byte[] boolBytes = MessagePackSerializer.Serialize(true);

        /// <summary>
        /// Test bytes for <see cref="char"/>
        /// </summary>
        private readonly byte[] charBytes = MessagePackSerializer.Serialize(char.MaxValue);

        /// <summary>
        /// Test bytes for <see cref="string"/>
        /// </summary>
        private readonly byte[] stringBytes = MessagePackSerializer.Serialize("Benchmark test");

        /// <summary>
        /// Test bytes for <see cref="DateTime"/>
        /// </summary>
        private readonly byte[] dateTimeBytes = MessagePackSerializer.Serialize(DateTime.UtcNow);

        /// <summary>
        /// Test bytes for <see cref="TimeSpan"/>
        /// </summary>
        private readonly byte[] timeSpanBytes = MessagePackSerializer.Serialize(TimeSpan.MaxValue);

        /// <summary>
        /// Test bytes for <see cref="Guid"/>
        /// </summary>
        private readonly byte[] guidBytes = MessagePackSerializer.Serialize(Guid.NewGuid());

        /// <summary>
        /// Deserialization <see cref="Guid"/> benchmark.
        /// </summary>
        [Benchmark]
        public void GuidBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(Guid), guidBytes);
            });
        }

        /// <summary>
        /// Deserialization <see cref="short"/> benchmark.
        /// </summary>
        [Benchmark]
        public void ShortBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(short), shortBytes);
            });
        }

        /// <summary>
        /// Deserialization <see cref="ushort"/> benchmark.
        /// </summary>
        [Benchmark]
        public void UshortBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(ushort), ushortBytes);
            });
        }

        /// <summary>
        /// Deserialization <see cref="int"/> benchmark.
        /// </summary>
        [Benchmark]
        public void IntBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(int), intBytes);
            });
        }

        /// <summary>
        /// Deserialization <see cref="uint"/> benchmark.
        /// </summary>
        [Benchmark]
        public void UintBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(uint), uintBytes);
            });
        }

        /// <summary>
        /// Deserialization <see cref="sbyte"/> benchmark.
        /// </summary>
        [Benchmark]
        public void SbyteBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(sbyte), sbyteBytes);
            });
        }

        /// <summary>
        /// Deserialization <see cref="byte"/> benchmark.
        /// </summary>
        [Benchmark]
        public void ByteBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(byte), byteBytes);
            });
        }

        /// <summary>
        /// Deserialization <see cref="bool"/> benchmark.
        /// </summary>
        [Benchmark]
        public void BoolBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(bool), boolBytes);
            });
        }

        /// <summary>
        /// Deserialization <see cref="char"/> benchmark.
        /// </summary>
        [Benchmark]
        public void CharBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(char), charBytes);
            });
        }

        /// <summary>
        /// Deserialization <see cref="string"/> benchmark.
        /// </summary>
        [Benchmark]
        public void StringBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(string), stringBytes);
            });
        }

        /// <summary>
        /// Deserialization <see cref="TimeSpan"/> benchmark.
        /// </summary>
        [Benchmark]
        public void TimeSpanBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(TimeSpan), timeSpanBytes);
            });
        }

        /// <summary>
        /// Deserialization <see cref="float"/> benchmark.
        /// </summary>
        [Benchmark]
        public void FloatBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(float), floatBytes);
            });
        }

        /// <summary>
        /// Deserialization <see cref="double"/> benchmark.
        /// </summary>
        [Benchmark]
        public void DoubleBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(double), doubleBytes);
            });
        }

        /// <summary>
        /// Deserialization <see cref="DateTime"/> benchmark.
        /// </summary>
        [Benchmark]
        public void DateTimeBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(DateTime), dateTimeBytes);
            });
        }

        /// <summary>
        /// Deserialization <see cref="long"/> benchmark.
        /// </summary>
        [Benchmark]
        public void LongBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(long), longBytes);
            });
        }

        /// <summary>
        /// Deserialization <see cref="ulong"/> benchmark.
        /// </summary>
        [Benchmark]
        public void UlongBenchmark()
        {
            RunInIteration(() =>
            {
                MessagePackSerializer.Deserialize(typeof(ulong), ulongBytes);
            });
        }
    }
}
