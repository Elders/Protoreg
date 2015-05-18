using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Elders.Protoreg.Protobuff
{
    [Serializable]
    public abstract class ContractException : Exception
    {
        public ContractException() { }
        public ContractException(string message) : base(message) { }
        public ContractException(string message, Exception inner) : base(message, inner) { }
        protected ContractException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class InvalidContractNameException : ContractException
    {

        public InvalidContractNameException(Type contract) : this(String.Format("Cannot register hash for contract {0}. Suggested Guid: {1}. If you use DataContractAttribute please check the protocol buffers specification defined here: https://developers.google.com/protocol-buffers/docs/proto and specifically 'The smallest tag number you can specify is 1, and the largest is 229 - 1, or 536,870,911'. You can also get a valid contract ID using 'ProtoRegistration.GenerateContractId();'", contract.FullName, ProtoRegistration.GenerateContractId())) { }
        public InvalidContractNameException(string message) : base(message) { }
        public InvalidContractNameException(string message, Exception inner) : base(message, inner) { }
        protected InvalidContractNameException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class DuplicateContractNameException : ContractException
    {
        public DuplicateContractNameException(Type contract) : this(String.Format("A duplicate runtime model field number detected for contract '{0}'. Suggested Guid: {1}. If you use DataContractAttribute please check the protocol buffers specification defined here: https://developers.google.com/protocol-buffers/docs/proto and specifically 'The smallest tag number you can specify is 1, and the largest is 229 - 1, or 536,870,911'. You can also get a valid contract ID using 'ProtoRegistration.GenerateContractId();'", contract.FullName, ProtoRegistration.GenerateContractId())) { }
        public DuplicateContractNameException(string message) : base(message) { }
        public DuplicateContractNameException(string message, Exception inner) : base(message, inner) { }
        protected DuplicateContractNameException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }
}