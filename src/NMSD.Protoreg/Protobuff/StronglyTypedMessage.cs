using System.Runtime.Serialization;

namespace NMSD.Protoreg.Protobuff
{
    [DataContract(Name = "e1eb04bd-d576-47d9-a8e8-aa7eb08e0c5c")]
    public class StronglyTypedMessage<T> : Message
    {
        public StronglyTypedMessage() { }

        [DataMember(Order = 1)]
        private T StronglyTypedBody
        {
            get { return (T)base.Body; }
            set { base.Body = value; }
        }
    }
}