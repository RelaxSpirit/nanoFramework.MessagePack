// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


#if NANOFRAMEWORK_1_0
using System;
using System.IO;
using nanoFramework.TestFramework;
#endif
using System.Collections;
using System.Diagnostics;
using nanoFramework.MessagePack;
using UnitTestShared.Helpers;
using UnitTestShared.TestData;

namespace NFUnitTest
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void TestConverterContext()
        {
            var converter = ConverterContext.GetConverter(typeof(bool));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(short));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(ushort));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(int));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(uint));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(long));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(ulong));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(byte));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(sbyte));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(float));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(double));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(string));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(TimeSpan));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(DateTime));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(char));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(Guid));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(byte[]));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(ArrayList));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(int[]));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(uint[]));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(long[]));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(ulong[]));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(float[]));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(double[]));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(char[]));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(string[]));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(Guid[]));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(sbyte[]));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(DateTime[]));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(TimeSpan[]));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(bool[]));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(short[]));
            Assert.IsNotNull(converter);
            converter = ConverterContext.GetConverter(typeof(ushort[]));
            Assert.IsNotNull(converter);

            Assert.IsNotNull(ConverterContext.NullConverter);

            var testConverter = TestsHelper.GetTestConverter();
            ConverterContext.Add(typeof(byte[][]), testConverter);
            converter = ConverterContext.GetConverter(typeof(byte[][]));
            Assert.IsNotNull(converter);
            ConverterContext.Replace(typeof(byte[][]), testConverter);
            converter = ConverterContext.GetConverter(typeof(byte[][]));
            Assert.IsNotNull(converter);
            ConverterContext.Remove(typeof(byte[][]));
            converter = ConverterContext.GetConverter(typeof(byte[][]));
            Assert.IsNull(converter);
#if NANOFRAMEWORK_1_0
            Assert.ThrowsException(typeof(NotSupportedException), () => ConverterContext.Add(typeof(object), testConverter));
#else
            Assert.ThrowsException<NotSupportedException>(() => ConverterContext.Add(typeof(object), testConverter));
#endif
        }

        [TestMethod]
        public void ProcessCustomObjectTest()
        {
            
            var test = TestsHelper.GetTestClassObject();
#if !NANOFRAMEWORK_1_0
            test.SubTestObject.NullableDateTime = DateTime.UtcNow;
#endif
            var resultBytes = MessagePackSerializer.Serialize(test);
            Debug.WriteLine($"Serialize byte size: {resultBytes.Length}");
            var testResult = (TestClass)MessagePackSerializer.Deserialize(typeof(TestClass), resultBytes)!;

            Assert.IsNotNull(testResult);
            Assert.AreEqual(test, testResult);

            using MemoryStream ms = new(resultBytes);

            var msTestResult = (TestClass)MessagePackSerializer.Deserialize(typeof(TestClass), ms)!;

            Assert.IsNotNull(msTestResult);
            Assert.AreEqual(testResult, msTestResult);

            using MemoryStream writeMemory = new();
            MessagePackSerializer.Serialize(msTestResult, writeMemory);
            byte[] testBytes = writeMemory.ToArray();

            Assert.AreNotEqual(resultBytes, testBytes);
            Assert.IsTrue(resultBytes.ArrayEqual(testBytes));

        }

        [TestMethod]
        public void CustomConverterTest()
        {
            var secureMessageConverter = new SecureMessageConverter();
            ConverterContext.Add(typeof(SecureMessage), secureMessageConverter);

            var secureMessage = new SecureMessage("Hello MessagePack at nanoFramework!");

            //At sender
            var buffer = MessagePackSerializer.Serialize(secureMessage);
            Debug.WriteLine($"The message:\n{secureMessage.Message}\nbeing sent has been serialized into {buffer.Length} bytes.");
            //and sent to recipient
            //
            //..........................
            Debug.WriteLine("=============================================");

            //At recipient, after receiving the byte array
            Debug.WriteLine($"Received {buffer.Length} bytes");

            var recipientSecureMessage = (SecureMessage)MessagePackSerializer.Deserialize(typeof(SecureMessage), buffer)!;

            Debug.WriteLine($"Message received:\n{recipientSecureMessage.Message}");

            Assert.AreNotEqual(secureMessage, recipientSecureMessage);

            Assert.AreEqual(secureMessage.Message, recipientSecureMessage.Message);
        }

        [TestMethod]
        public void PrimitivesTest()
        {
            byte[] shortBytes = MessagePackSerializer.Serialize(short.MaxValue);
            var shortValue = (short)MessagePackSerializer.Deserialize(typeof(short), shortBytes)!;
            Assert.AreEqual(short.MaxValue, shortValue);

            byte[] ushortBytes = MessagePackSerializer.Serialize(ushort.MaxValue);
            var ushortValue = (ushort)MessagePackSerializer.Deserialize(typeof(ushort), ushortBytes)!;
            Assert.AreEqual(ushort.MaxValue, ushortValue);

            byte[] intBytes = MessagePackSerializer.Serialize(int.MaxValue);
            var intValue = (int)MessagePackSerializer.Deserialize(typeof(int), intBytes)!;
            Assert.AreEqual(int.MaxValue, intValue);

            byte[] uintBytes = MessagePackSerializer.Serialize(uint.MaxValue);
            var uintValue = (uint)MessagePackSerializer.Deserialize(typeof(uint), uintBytes)!;
            Assert.AreEqual(uint.MaxValue, uintValue);

            byte[] longBytes = MessagePackSerializer.Serialize(long.MaxValue);
            var longValue = (long)MessagePackSerializer.Deserialize(typeof(long), longBytes)!;
            Assert.AreEqual(long.MaxValue, longValue);

            byte[] ulongBytes = MessagePackSerializer.Serialize(ulong.MaxValue);
            var ulongValue = (ulong)MessagePackSerializer.Deserialize(typeof(ulong), ulongBytes)!;
            Assert.AreEqual(ulong.MaxValue, ulongValue);

            byte[] byteBytes = MessagePackSerializer.Serialize(byte.MaxValue);
            var byteValue = (byte)MessagePackSerializer.Deserialize(typeof(byte), byteBytes)!;
            Assert.AreEqual(byte.MaxValue, byteValue);

            byte[] sbyteBytes = MessagePackSerializer.Serialize(sbyte.MaxValue);
            var sbyteValue = (sbyte)MessagePackSerializer.Deserialize(typeof(sbyte), sbyteBytes)!;
            Assert.AreEqual(sbyte.MaxValue, sbyteValue);

            byte[] floatBytes = MessagePackSerializer.Serialize(float.MaxValue);
            var floatValue = (float)MessagePackSerializer.Deserialize(typeof(float), floatBytes)!;
            Assert.AreEqual(float.MaxValue, floatValue);

            byte[] doubleBytes = MessagePackSerializer.Serialize(double.MaxValue);
            var doubleValue = (double)MessagePackSerializer.Deserialize(typeof(double), doubleBytes)!;
            Assert.AreEqual(double.MaxValue, doubleValue);

            byte[] boolBytes = MessagePackSerializer.Serialize(true);
            var boolValue = (bool)MessagePackSerializer.Deserialize(typeof(bool), boolBytes)!;
            Assert.AreEqual(true, boolValue);

            boolBytes = MessagePackSerializer.Serialize(false);
            boolValue = (bool)MessagePackSerializer.Deserialize(typeof(bool), boolBytes)!;
            Assert.AreEqual(false, boolValue);

            byte[] charBytes = MessagePackSerializer.Serialize(char.MaxValue);
            var charValue = (char)MessagePackSerializer.Deserialize(typeof(char), charBytes)!;
            Assert.AreEqual(char.MaxValue, charValue);

            string testString = "I'm a UnitTest string!";
            byte[] stringBytes = MessagePackSerializer.Serialize(testString);
            var stringValue = (string)MessagePackSerializer.Deserialize(typeof(string), stringBytes)!;
            Assert.AreEqual(testString, stringValue);

            DateTime testDateTime = DateTime.UtcNow - TimeSpan.FromHours(1);
            byte[] dateTimeBytes = MessagePackSerializer.Serialize(testDateTime);
            var dateTimeValue = (DateTime)MessagePackSerializer.Deserialize(typeof(DateTime), dateTimeBytes)!;
            Assert.AreEqual(testDateTime, dateTimeValue);

            byte[] timeSpanBytes = MessagePackSerializer.Serialize(TimeSpan.MaxValue);
            var timeSpanValue = (TimeSpan)MessagePackSerializer.Deserialize(typeof(TimeSpan), timeSpanBytes)!;
            Assert.AreEqual(TimeSpan.MaxValue, timeSpanValue);

            var testGuid = Guid.NewGuid();
            byte[] guidBytes = MessagePackSerializer.Serialize(testGuid);
            var guidValue = (Guid)MessagePackSerializer.Deserialize(typeof(Guid), guidBytes)!;
            Assert.AreEqual(testGuid, guidValue);

        }

        [TestMethod]
        public void ArrayAndHashtableTest()
        {
            int[] intArray = new int[10]
            {
                int.MaxValue,
                int.MaxValue / 2,
                int.MaxValue / 3,
                int.MaxValue - 100,
                -236499102,
                int.MinValue,
                -3,
                -98747483,
                239390494,
                1
            };

            var arrayBytes = MessagePackSerializer.Serialize(intArray);
            var intArrayValue = (int[])MessagePackSerializer.Deserialize(typeof(int[]), arrayBytes)!;
            Assert.IsTrue(intArray.ArrayEqual(intArrayValue));

            byte[] byteArray = new byte[10]
            {
                100,
                111,
                byte.MaxValue,
                126,
                45,
                20,
                byte.MinValue,
                2,
                1,
                10
            };

            arrayBytes = MessagePackSerializer.Serialize(byteArray);
            var byteArrayValue = (byte[])MessagePackSerializer.Deserialize(typeof(byte[]), arrayBytes)!;
            Assert.IsTrue(byteArray.ArrayEqual(byteArrayValue));

            long[][] twoDimensionalArray = new long[][]
            {
                new long[]{1, 2, 3, 4, 5},
                new long[]{long.MaxValue -1, -1, -3985948353983, -8923879},
                new long[]{8734832748327, -84393275329, 87348932758, long.MinValue},
            };
            arrayBytes = MessagePackSerializer.Serialize(twoDimensionalArray);
            var twoDimensionalArrayValue = (long[][])MessagePackSerializer.Deserialize(typeof(long[][]), arrayBytes)!;
            Assert.IsTrue(twoDimensionalArray.CheckTwoDimensionalLongArray(twoDimensionalArrayValue, out string errorMessage), errorMessage);

            Hashtable testHashtable = new()
            {
                { "gender", "Male" },
                { "snow", "white" },
                { Guid.NewGuid().ToString(), "nanoFramework.MessagePack" }
            };
            arrayBytes = MessagePackSerializer.Serialize(testHashtable);
            var hashtableValue = (Hashtable)MessagePackSerializer.Deserialize(typeof(Hashtable), arrayBytes)!;
            Assert.AreEqual(testHashtable.Count, hashtableValue.Count);
            foreach (DictionaryEntry entry in testHashtable)
            {
                Assert.AreEqual(entry.Value, hashtableValue[entry.Key]);
            }
#if !NANOFRAMEWORK_1_0
            Dictionary<string, string> testDictionary = new Dictionary<string, string>()
            {
                {"test", "value" }
            };

            arrayBytes = MessagePackSerializer.Serialize(testDictionary);
            var resultDictionary = (Dictionary<string, string>)MessagePackSerializer.Deserialize(typeof(Dictionary<string, string>), arrayBytes)!;
            Assert.AreEqual(testDictionary.Count, resultDictionary.Count);
            foreach (KeyValuePair<string, string> entry in testDictionary)
            {
                Assert.AreEqual(entry.Value, resultDictionary[entry.Key]);
            }

            List<int> testList = [1234, 1313131, 311313122];
            arrayBytes = MessagePackSerializer.Serialize(testList);
            var resultList = (List<int>)MessagePackSerializer.Deserialize(typeof(List<int>), arrayBytes)!;
            Assert.AreEqual(testList.Count, resultList.Count);

            for(int i = 0; i < testList.Count; i++)
            {
                Assert.AreEqual(testList[i], resultList[i]);
            }
#endif
        }
    }
}
