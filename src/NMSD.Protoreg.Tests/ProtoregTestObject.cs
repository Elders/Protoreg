using System;
using System.Runtime.Serialization;

namespace NMSD.Protoreg.Tests
{
    [DataContract(Name = "42dd25bd-e3f4-42a5-9c6d-6a3f831c33e4")]
    public class ProtoregTestObject
    {
        ProtoregTestObject() { }

        public ProtoregTestObject(Guid guidProperty, string stringProperty, int intProperty, long longProperty)
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