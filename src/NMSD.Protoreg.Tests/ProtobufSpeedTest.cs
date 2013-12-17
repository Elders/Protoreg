using System;
using System.IO;
using NMSD.Protoreg.Protobuff;

namespace NMSD.Protoreg.Tests
{
    public class ProtobufSpeedTest
    {
        StronglyTypedMessage<ProtobufTestObject> testObject;
        byte[] testObjectBytes;

        public ProtobufSpeedTest()
        {
            testObject = new StronglyTypedMessage<ProtobufTestObject>();
            testObject.Body = new ProtobufTestObject(Guid.NewGuid(), "string", Int32.MaxValue, long.MaxValue);
            testObjectBytes = Serialize(testObject);
        }

        public void TestSerialization(int numberOfExecutions)
        {
            var results = MeasureExecutionTime.Start(() =>
            {
                Serialize(testObject);
            }, numberOfExecutions);
            Console.WriteLine("Serialization of {0} objects with Protobuf took:", numberOfExecutions);
            Console.WriteLine(results);
        }

        public void TestDeserialization(int numberOfExecutions)
        {
            var results = MeasureExecutionTime.Start(() =>
            {
                Deserialize(testObjectBytes);
            }, numberOfExecutions);
            Console.WriteLine("Deserialize of {0} objects with Protobuf took:", numberOfExecutions);
            Console.WriteLine(results);
        }

        byte[] Serialize(object obj)
        {
            using (var stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, testObject);
                return stream.ToArray();
            }
        }

        StronglyTypedMessage<ProtobufTestObject> Deserialize(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                return ProtoBuf.Serializer.Deserialize<StronglyTypedMessage<ProtobufTestObject>>(stream);
            }
        }

    }
}