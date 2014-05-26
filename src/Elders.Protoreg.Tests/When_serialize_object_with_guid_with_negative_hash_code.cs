using Elders.Protoreg.Protobuff;
using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Elders.Protoreg.Tests
{
    [Subject("Serialization with negative hash code")]
    public class When_serialize_object_with_guid_with_negative_hash_code
    {
        Establish context = () =>
        {
            var protoReg = new ProtoRegistration();
            protoReg.RegisterCommonType<ProtoregTestObjectWithNegativeHashCode>();
            serializer = new ProtoregSerializer(protoReg);
            serializer.Build();

            protoregTestObject = new ProtoregTestObjectWithNegativeHashCode(Guid.NewGuid(), "string", int.MaxValue, long.MaxValue);
            protoregTestObjectBytes = Serialize(protoregTestObject);
        };

        Because of = () => protoregTestObjectDeserialized = Deserialize(protoregTestObjectBytes);

        It deserialized_object_should_not_be_null = () => protoregTestObjectDeserialized.ShouldNotBeNull();

        It deserialized_object_should_be_of_the_correct_type = () => protoregTestObjectDeserialized.ShouldBeOfType<ProtoregTestObjectWithNegativeHashCode>();

        It should_have_deserealized_property_of_type_Guid = () => protoregTestObjectDeserialized.GuidProperty.ShouldBeOfType<Guid>();

        It should_have_deserealized_property_of_type_String = () => protoregTestObjectDeserialized.StringProperty.ShouldBeOfType<string>();

        It should_have_deserealized_property_of_type_Int = () => protoregTestObjectDeserialized.IntProperty.ShouldBeOfType<int>();

        It should_have_deserealized_property_of_type_Long = () => protoregTestObjectDeserialized.LongProperty.ShouldBeOfType<long>();

        static byte[] protoregTestObjectBytes;
        static ProtoregTestObjectWithNegativeHashCode protoregTestObject;
        static ProtoregTestObjectWithNegativeHashCode protoregTestObjectDeserialized;
        static ProtoregSerializer serializer;

        static byte[] Serialize(object obj)
        {
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, protoregTestObject);
                return stream.ToArray();
            }
        }

        static ProtoregTestObjectWithNegativeHashCode Deserialize(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                return serializer.Deserialize(stream) as ProtoregTestObjectWithNegativeHashCode;
            }
        }
    }

    [DataContract(Name = "525ec964-a5f0-42eb-993d-77c12ad27a2b")]
    public class ProtoregTestObjectWithNegativeHashCode
    {
        ProtoregTestObjectWithNegativeHashCode() { }

        public ProtoregTestObjectWithNegativeHashCode(Guid guidProperty, string stringProperty, int intProperty, long longProperty)
        {
            GuidProperty = guidProperty;
            StringProperty = stringProperty;
            IntProperty = intProperty;
            LongProperty = longProperty;
        }

        [DataMember(Order = 1)]
        public Guid GuidProperty { get; set; }

        [DataMember(Order = 2)]
        public string StringProperty { get; set; }

        [DataMember(Order = 3)]
        public int IntProperty { get; set; }

        [DataMember(Order = 4)]
        public long LongProperty { get; set; }
    }
}