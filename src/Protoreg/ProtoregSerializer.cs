using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LaCore.Hyperion.Infrastructure.MassTransit.Protobuff;

namespace LaCore.Hyperion.Infrastructure.MassTransit
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
            object obj = (serializer.Deserialize(stream) as List<Message>).First().Body;
            return obj;
        }

        public void Serialize<T>(Stream stream, T @object) where T : class
        {
            Message msg = new Message();
            msg.Body = (T)@object;
            serializer.Serialize(stream, new List<Message>() { msg });
        }

        public void Build()
        {
            serializer = new ProtocolBufferSerializer(registrations.GetRegisteredTypes());
        }
    }
}
