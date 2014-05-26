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
    [Subject("Serialization with hash code between 19000 and 19999")]
    public class When_serialize_object_with_guid_with_hash_code_between_19000_and_19999
    {
        Establish context = () =>
        {
            var protoReg = new ProtoRegistration();
            protoReg.RegisterCommonType<ProtoregTestObjectWithHashCodeBetween19000And19999>();
            serializer = new ProtoregSerializer(protoReg);
        };

        Because of = () => exception = Catch.Exception(() => serializer.Build());

        It should_throw_an_exception = () => exception.ShouldBeOfType<InvalidContractNameException>();

        static ProtoregSerializer serializer;
        static Exception exception;
    }

    [DataContract(Name = "73226d2f-f823-472a-975a-8b1b26982bdd")]
    public class ProtoregTestObjectWithHashCodeBetween19000And19999
    {
        ProtoregTestObjectWithHashCodeBetween19000And19999() { }

        public ProtoregTestObjectWithHashCodeBetween19000And19999(Guid guidProperty, string stringProperty, int intProperty, long longProperty)
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