using System;

namespace Elders.Protoreg.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            int numberOfExecutions = 1000000;

            ProtobufSpeedTest protobuf = new ProtobufSpeedTest();
            ProtoregSpeedTest protoreg = new ProtoregSpeedTest();

            protoreg.TestSerialization(numberOfExecutions);
            protobuf.TestSerialization(numberOfExecutions);

            protoreg.TestDeserialization(numberOfExecutions);
            protobuf.TestDeserialization(numberOfExecutions);

            Console.ReadLine();
        }
    }
}