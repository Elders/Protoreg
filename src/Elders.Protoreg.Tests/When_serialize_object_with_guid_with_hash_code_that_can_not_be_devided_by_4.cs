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
    [Subject("Serialization with hash code that can not be devided by 4")]
    public class When_serialize_object_with_guid_with_hash_code_that_can_not_be_devided_by_4
    {
        Establish context = () =>
        {
            var protoReg = new ProtoRegistration();
            protoReg.RegisterCommonType<ProtoregTestObjectWithHashCodeThatCanNotBeDevidedBy4>();
            serializer = new ProtoregSerializer(protoReg);
        };

        Because of = () => exception = Catch.Exception(() => serializer.Build());

        It should_throw_an_exception = () => exception.ShouldBeOfType<InvalidContractNameException>();

        static ProtoregSerializer serializer;
        static Exception exception;
    }
    
    [DataContract(Name = "5e9bf1b0-5189-4e91-8165-63ccae7c822e")]
    public class ProtoregTestObjectWithHashCodeThatCanNotBeDevidedBy4
    {
        ProtoregTestObjectWithHashCodeThatCanNotBeDevidedBy4() { }

        public ProtoregTestObjectWithHashCodeThatCanNotBeDevidedBy4(Guid guidProperty, string stringProperty, int intProperty, long longProperty)
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