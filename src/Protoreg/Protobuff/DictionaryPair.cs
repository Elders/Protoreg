using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LaCore.Hyperion.Infrastructure.MassTransit.Protobuff
{
    [DataContract]
    public class DictionaryPair
    {
        DictionaryPair() { }

        public DictionaryPair(string key, string value)
        {
            Key = key;
            Value = value;
        }

        [DataMember(Order = 1)]
        public string Key { get; set; }
        [DataMember(Order = 2)]
        public string Value { get; set; }
    }
}
