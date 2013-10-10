using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LaCore.Hyperion.Infrastructure.MassTransit.Protobuff
{
    [DataContract]
    public class StronglyTypedMessage<T> : Message
    {
        StronglyTypedMessage()
        {
            DenormalizedDictionary = new List<DictionaryPair>();
        }

        public StronglyTypedMessage(Dictionary<string, object> headers, object body)
        {
            DenormalizedDictionary = headers.Select(x => new DictionaryPair(x.Key, x.Value.ToString())).ToList();
            StronglyTypedBody = (T)body;
        }

        [DataMember(Order = 1)]
        public List<DictionaryPair> DenormalizedDictionary { get; set; }

        [DataMember(Order = 2)]
        private T StronglyTypedBody
        {
            get { return (T)base.Body; }
            set { base.Body = value; }
        }

        public Message ToEventMessage()
        {
            Message result = this;
            DenormalizedDictionary.ForEach(pair => result.Headers.Add(pair.Key, pair.Value as object));
            return result;
        }
    }



}
