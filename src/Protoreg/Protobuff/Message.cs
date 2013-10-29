using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Protoreg.Protobuff
{
    [DataContract, Serializable]
    public class Message
    {
        public Message()
        {
            this.Headers = new Dictionary<string, object>();
        }
        [DataMember]
        public Dictionary<string, object> Headers { get; private set; }

        [DataMember]
        public object Body { get; set; }
    }
}
