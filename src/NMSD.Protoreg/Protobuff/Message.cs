using System;
using System.Runtime.Serialization;

namespace NMSD.Protoreg.Protobuff
{
    [DataContract(Name = "e39c7d44-2bd0-4f17-9b3d-71ca9678a7db"), Serializable]
    public class Message
    {
        public Message() { }

        [DataMember(Order = 1)]
        public object Body { get; set; }
    }
}