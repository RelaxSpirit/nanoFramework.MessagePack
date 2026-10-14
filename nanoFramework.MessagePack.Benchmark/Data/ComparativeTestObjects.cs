// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using nanoFramework.Json;

namespace nanoFramework.MessagePack.Benchmark.Data
{
    /// <summary>
    /// Test data.
    /// </summary>
    internal class ComparativeTestObjects
    {
        /// <summary>
        /// Test json text.
        /// </summary>
        internal const string TestJson = @"{
  ""StartDate"": ""2026-01-27T00:00:00.0000000Z"",
  ""WorkingTime"": ""00:02:00"",
  ""TestObjects"": [
    {
      ""UID"": ""e8048178-b787-4f22-85bc-891b6637d353"",
      ""Id"": 1
    },
    {
      ""UID"": ""41B07FC4-AE08-4F9B-B9A7-27E8877708D2"",
      ""Id"": 2
    }
  ],
  ""IntArrayData"": [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ],
  ""Users"": [
    {
      ""Role"": {
        ""Name"": ""Admin"",
        ""Id"": 1
      },
      ""IsActive"": true,
      ""Name"": ""Test""
    }
  ],
  ""HashData"": {
    ""a"": ""Test"",
    ""b"": 123
  }
}";

        /// <summary>
        /// Gets json deserialize option.
        /// </summary>
        internal static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        internal ComparativeTestObjects()
        {
            TestObject = (ComparativeBenchmarkObject)JsonConvert.DeserializeObject(TestJson, typeof(ComparativeBenchmarkObject), JsonSerializerOptions);
            TestObjectMsgPackBytes = MessagePackSerializer.Serialize(TestObject);
            TestObjectBinaryBytes = BinaryFormatter.Serialize(TestObject);
        }

        /// <summary>
        /// Gets test object for deserialize <see cref="TestJson"/>.
        /// </summary>
        internal static ComparativeBenchmarkObject TestObject { get; private set; }

        /// <summary>
        /// Gets byte array for test object used by <see cref="MessagePack"/>.
        /// </summary>
        internal static byte[] TestObjectMsgPackBytes { get; private set; }

        /// <summary>
        /// Gets byte array for test object used by <see cref="BinaryFormatter"/>.
        /// </summary>
        internal static byte[] TestObjectBinaryBytes { get; private set; }

        /// <summary>
        /// Dummy main test serialization object class.
        /// </summary>
        [Serializable]
        public class ComparativeBenchmarkObject
        {
            /// <summary>
            /// Gets or sets test property. Not used.
            /// </summary>
            public DateTime StartDate { get; set; }

            /// <summary>
            /// Gets or sets test property. Not used.
            /// </summary>
            public TimeSpan WorkingTime { get; set; }

            /// <summary>
            /// Gets or sets test property. Not used.
            /// </summary>
            public int[] IntArrayData { get; set; }

            /// <summary>
            /// Gets or sets test property. Not used.
            /// </summary>
            public Hashtable HashData { get; set; }

            /// <summary>
            /// Gets or sets test property. Not used.
            /// </summary>
            public User[] Users { get; set; }

            /// <summary>
            /// Gets or sets test property. Not used.
            /// </summary>
            public BenchmarkTestObject[] TestObjects { get; set; }
        }

        /// <summary>
        /// Dummy main test serialization object class.
        /// </summary>
        [Serializable]
        public class BenchmarkTestObject
        {
            /// <summary>
            /// Gets or sets test property. Not used.
            /// </summary>
            public ulong Id { get; set; }

            /// <summary>
            /// Gets or sets test property. Not used.
            /// </summary>
            public Guid UID { get; set; }
        }

        /// <summary>
        /// Dummy main test serialization object class.
        /// </summary>
        [Serializable]
        public class User
        {
            /// <summary>
            /// Gets or sets test property. Not used.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets test property. Not used.
            /// </summary>
            public bool IsActive { get; set; }

            /// <summary>
            /// Gets or sets test property. Not used.
            /// </summary>
            public Role Role { get; set; }
        }

        /// <summary>
        /// Dummy main test serialization object class.
        /// </summary>
        [Serializable]
        public class Role
        {
            /// <summary>
            /// Gets or sets test property. Not used.
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Gets or sets test property. Not used.
            /// </summary>
            public string Name { get; set; }
        }
    }
}
