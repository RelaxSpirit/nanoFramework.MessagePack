// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using nanoFramework.Benchmark;
using nanoFramework.Json;
using nanoFramework.MessagePack.Benchmark.Data;
using nanoFramework.MessagePack.Benchmark.DeserializationBenchmarks;
using nanoFramework.MessagePack.Benchmark.SerializationBenchmarks;

namespace nanoFramework.MessagePack.Benchmark
{
    /// <summary>
    /// Main program
    /// </summary>
    internal class Program
    {
        private const int BannerWidth = 63;
        private const int BannerPadWidth = 10;
        private const int SubBannerWidth = 43;
        
        /// <summary>
        /// Enter point for run all benchmarks
        /// </summary>
        internal static void Main()
        {
            string starPad = new string('*', BannerPadWidth);
            Console.WriteLine($"{starPad} Starting benchmarks on {Runtime.Native.SystemInfo.TargetName} {starPad}");
            BenchmarkRunner.RunClass(typeof(ReferenceTypesDeserializationBenchmark));
            BenchmarkRunner.RunClass(typeof(ValueTypesDeserializationBenchmark));
            BenchmarkRunner.RunClass(typeof(ReferenceTypesSerializationBenchmark));
            BenchmarkRunner.RunClass(typeof(ValueTypesSerializationBenchmark));
            BenchmarkRunner.RunClass(typeof(TypeCompareBenchmark));

            var testData = new ComparativeTestObjects();
            string bannerTopBottomLine = new string('=', BannerWidth);
            string bannerPad = new string('=', BannerPadWidth);
            string subBannerLine = new string(' ', SubBannerWidth);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(bannerTopBottomLine);
            Console.WriteLine($"{bannerPad}         Comparative benchmarks data       {bannerPad}");
            Console.WriteLine($"{bannerPad}{subBannerLine}{bannerPad}");
            Console.WriteLine($"{bannerPad} Json string size:           {JsonConvert.SerializeObject(ComparativeTestObjects.TestObject).Length} bytes     {bannerPad}");
            Console.WriteLine($"{bannerPad} MessagePack array size:     {ComparativeTestObjects.TestObjectMsgPackBytes.Length} bytes     {bannerPad}");
            Console.WriteLine($"{bannerPad} BinaryFormatter array size: {ComparativeTestObjects.TestObjectBinaryBytes.Length} bytes     {bannerPad}");
            Console.WriteLine($"{bannerPad}{subBannerLine}{bannerPad}");
            Console.WriteLine(bannerTopBottomLine);

            BenchmarkRunner.RunClass(typeof(ComparativeDeserializationBenchmark));
            BenchmarkRunner.RunClass(typeof(ComparativeSerializationBenchmark));
            Console.WriteLine($"{starPad} Completed benchmarks {starPad}");

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
