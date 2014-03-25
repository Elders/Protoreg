using System;
using System.IO;
using Machine.Specifications;
using Elders.Protoreg;

namespace Elders.Protoreg.Tests
{
    [Subject("Serialization")]
    public class When_serialize_object
    {
        Establish context = () =>
        {
            var protoReg = new ProtoRegistration();
            protoReg.RegisterAssembly<ProtoregTestObject>();
            serializer = new ProtoregSerializer(protoReg);
            serializer.Build();

            protoregTestObject = new ProtoregTestObject(Guid.NewGuid(), "string", int.MaxValue, long.MaxValue);
            protoregTestObjectBytes = Serialize(protoregTestObject);

        };

        Because of = () => protoregTestObjectDeserialized = Deserialize(protoregTestObjectBytes);

        It deserialized_object_should_not_be_null = () => protoregTestObjectDeserialized.ShouldNotBeNull();

        It deserialized_object_should_be_of_the_correct_type = () => protoregTestObjectDeserialized.ShouldBeOfType<ProtoregTestObject>();

        It should_have_deserealized_property_of_type_Guid = () => protoregTestObjectDeserialized.GuidProperty.ShouldBeOfType<Guid>();

        It should_have_deserealized_property_of_type_String = () => protoregTestObjectDeserialized.StringProperty.ShouldBeOfType<string>();

        It should_have_deserealized_property_of_type_Int = () => protoregTestObjectDeserialized.IntProperty.ShouldBeOfType<int>();

        It should_have_deserealized_property_of_type_Long = () => protoregTestObjectDeserialized.LongProperty.ShouldBeOfType<long>();

        static byte[] protoregTestObjectBytes;
        static ProtoregTestObject protoregTestObject;
        static ProtoregTestObject protoregTestObjectDeserialized;
        static ProtoregSerializer serializer;

        static byte[] Serialize(object obj)
        {
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, protoregTestObject);
                return stream.ToArray();
            }
        }

        static ProtoregTestObject Deserialize(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                return serializer.Deserialize(stream) as ProtoregTestObject;
            }
        }
    }
}
