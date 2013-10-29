using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Protoreg.Protobuff;

namespace Protoreg
{
    public class ProtoregSerializer
    {
        private readonly ProtoRegistration registrations;
        ProtocolBufferSerializer serializer;

        public ProtoregSerializer(ProtoRegistration registrations)
        {
            this.registrations = registrations;
        }

        public object Deserialize(Stream stream)
        {
            //object obj = (serializer.Deserialize(stream) as List<Message>).First().Body;
            //return obj;
            var deser = (serializer.Deserialize(stream) as List<Message>);
            if (deser == null || deser.Count == 0)
                return null;
            else if (deser.Count == 1)
                return deser.First().Body;
            else
                return deser.Select(x => x.Body).ToList();
        }

        public void Serialize<T>(Stream stream, T @object)
        {
            var msgs = new List<Message>();
            if (@object is IList)
            {
                foreach (dynamic item in (@object as IList))
                {
                    Message msg = new Message();
                    msg.Body = item;
                    msgs.Add(msg);
                }
            }
            else
            {
                Message msg = new Message();
                msg.Body = (T)@object;
                msgs.Add(msg);
            }
            serializer.Serialize(stream, msgs);
        }

        public void Build()
        {
            serializer = new ProtocolBufferSerializer(registrations.GetRegisteredTypes());
        }
    }
}
