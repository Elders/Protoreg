using System;
using System.Collections;
using System.IO;
using NMSD.Protoreg.Protobuff;

namespace NMSD.Protoreg
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
            var message = serializer.Deserialize(stream) as Message;
            if (message != null)
                return message.Body;
            else
                return null;
        }

        public void Serialize<T>(Stream stream, T @object)
        {
            if (@object is IList) throw new Exception("Wrap the list in a new object");

            Message msg = new Message();
            msg.Body = (T)@object;
            serializer.Serialize(stream, msg);
        }

        public void Build()
        {
            serializer = new ProtocolBufferSerializer(registrations.GetRegisteredTypes());
        }
    }
}