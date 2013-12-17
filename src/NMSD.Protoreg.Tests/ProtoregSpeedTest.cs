using System;
using System.IO;
using NMSD.Protoreg;

namespace NMSD.Protoreg.Tests
{
    public class ProtoregSpeedTest
    {
        ProtoregSerializer serializer;
        ProtoregTestObject testObject;
        byte[] testObjectBytes;

        public ProtoregSpeedTest()
        {
            var protoReg = new ProtoRegistration();
            protoReg.RegisterCommonType<ProtoregTestObject>();
            serializer = new ProtoregSerializer(protoReg);
            serializer.Build();

            testObject = new ProtoregTestObject(Guid.NewGuid(), "string", Int32.MaxValue, long.MaxValue);
            testObjectBytes = Serialize(testObject);
        }

        public void TestSerialization(int numberOfExecutions)
        {
            var results = MeasureExecutionTime.Start(() =>
            {
                Serialize(testObject);
            }, numberOfExecutions);
            Console.WriteLine("Serialization of {0} objects with Protoreg took:", numberOfExecutions);
            Console.WriteLine(results);
        }

        public void TestDeserialization(int numberOfExecutions)
        {
            var results = MeasureExecutionTime.Start(() =>
            {
                Deserialize(testObjectBytes);
            }, numberOfExecutions);
            Console.WriteLine("Deserialize of {0} objects with Protoreg took:", numberOfExecutions);
            Console.WriteLine(results);
        }

        byte[] Serialize(object obj)
        {
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, testObject);
                return stream.ToArray();
            }
        }

        ProtoregTestObject Deserialize(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                return serializer.Deserialize(stream) as ProtoregTestObject;
            }
        }

    }
}