// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;

namespace nanoFramework.MessagePack.Benchmark.Data
{
    /// <summary>
    /// Test references objects.
    /// </summary>
    internal class ReferenceTestObjects
    {
        /// <summary>
        /// Initializes static members of the <see cref="ReferenceTestObjects" /> class.
        /// </summary>
        static ReferenceTestObjects()
        {
            IntArray = new int[100];
            ByteArray = new byte[100];
            TwoDimensionalArray = new long[10][];
            TestHashtable = new Hashtable();

            var random = new Random();

            for (int i = 0; i < IntArray.Length; i++)
            {
                IntArray[i] = random.Next(int.MaxValue);
            }

            for (int i = 0; i < IntArray.Length; i++)
            {
                ByteArray[i] = (byte)random.Next(byte.MaxValue);
            }

            for (int x = 0; x < TwoDimensionalArray.Length; x++)
            {
                TwoDimensionalArray[x] = new long[10];
                for (int y = 0; y < TwoDimensionalArray[x].Length; y++)
                {
                    TwoDimensionalArray[x][y] = (long)random.Next(int.MaxValue) * int.MaxValue;
                }
            }

            TestHashtable.Add("gender", "Male");
            TestHashtable.Add("snow", "white");
            TestHashtable.Add(1, "one");
            TestHashtable.Add(2, "two");
            TestHashtable.Add(Guid.NewGuid().ToString(), new string[] { "nano,", "Framework", "Message", "Pack" });
        }

        /// <summary>
        /// Gets test int array.
        /// </summary>
        internal static int[] IntArray { get; }

        /// <summary>
        /// Gets test byte array.
        /// </summary>
        internal static byte[] ByteArray { get; }

        /// <summary>
        /// Gets test two-dimensional long array.
        /// </summary>
        internal static long[][] TwoDimensionalArray { get; }

        /// <summary>
        /// Gets test hashtable.
        /// </summary>
        internal static Hashtable TestHashtable { get; }
    }
}
