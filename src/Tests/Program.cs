using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Protoreg;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var protoReg = new ProtoRegistration();
            protoReg.RegisterAssembly<Program>();
            var serializer = new ProtoregSerializer(protoReg);
            serializer.Build();
            using (var stream = new MemoryStream())
            {
                var obj = new MyClass(1, "2", new Dictionary<string, string>() { { "a", "b" } });
                serializer.Serialize(stream, obj);
                stream.Position = 0;
                MyClass deserObj = serializer.Deserialize(stream) as MyClass;
                deserObj.MyString = "3";
                Console.WriteLine(deserObj);
                int i = 0;
                i++;
            }

        }

        [DataContract(Name = "aaa3cacc-a951-4d99-8580-f7f3e2059915")]
        public class MyClass
        {
            public MyClass() { }

            public MyClass(int myProperty, string myString, Dictionary<string, string> myStrings)
            {
                MyProperty = myProperty;
                MyString = myString;
                MyStrings = myStrings;
            }
            [DataMember(Order = 1)]
            public int MyProperty { get; set; }

            [DataMember(Order = 2)]
            public string MyString { get; set; }

            [DataMember(Order = 3)]
            public Dictionary<string, string> MyStrings { get; set; }
        }
    }
}
