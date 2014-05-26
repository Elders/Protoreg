using System;
using System.Runtime.Serialization;

namespace Elders.Protoreg
{
    [DataContract(Name = "c3ca519f-5ee8-460e-8f7b-c8a84d2fd191")]
    public class ProtoregSerializableException : Exception
    {
        ProtoregSerializableException() { }

        public ProtoregSerializableException(Exception ex)
        {
            ExType = ex.GetType();
            ExMessage = ex.Message;
            ExStackTrace = ex.StackTrace;
            //ExTargetSite = ex.TargetSite.ToString();
            ExSource = ex.Source;
            ExHelpLink = ex.HelpLink;

            if (ex.InnerException != null)
                ExInnerException = new ProtoregSerializableException(ex.InnerException);
        }

        [DataMember(Order = 1)]
        public Type ExType { get; private set; }

        [DataMember(Order = 2)]
        public string ExMessage { get; private set; }

        [DataMember(Order = 3)]
        public string ExStackTrace { get; private set; }
        //public string ExTargetSite { get; private set; }

        [DataMember(Order = 4)]
        public string ExSource { get; private set; }

        [DataMember(Order = 5)]
        public string ExHelpLink { get; private set; }

        [DataMember(Order = 100)]
        public ProtoregSerializableException ExInnerException { get; private set; }
    }
}
