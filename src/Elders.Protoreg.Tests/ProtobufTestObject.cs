using System;
using System.Runtime.Serialization;

namespace Elders.Protoreg.Tests
{
    [DataContract(Name = "76d61c8c-93bc-44b1-bee8-5e41b0abc7fd")]
    public class ProtobufTestObject
    {
        ProtobufTestObject() { }

        public ProtobufTestObject(Guid guidProperty, string stringProperty, int intProperty, long longProperty)
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