using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Elders.Protoreg.Protobuff;

namespace Elders.Protoreg
{
    public class ProtoRegistration
    {
        private HashSet<Assembly> assemblies = new HashSet<Assembly>();

        private HashSet<Type> commonTypes = new HashSet<Type>();

        public void RegisterAssembly(Assembly assembly)
        {
            assemblies.Add(assembly);
        }

        public void RegisterAssembly(Type typeInAssembly)
        {
            RegisterAssembly(Assembly.GetAssembly(typeInAssembly));
        }

        public void RegisterAssembly<T>()
        {
            RegisterAssembly(Assembly.GetAssembly(typeof(T)));
        }

        public void RegisterCommonType(Type type)
        {
            commonTypes.Add(type);
        }
        public void RegisterCommonType<T>()
        {
            RegisterCommonType(typeof(T));
        }

        public Type[] GetRegisteredTypes()
        {
            var contratAssemblies = new List<Assembly>(assemblies);
            var pbbAsm = Assembly.GetAssembly(typeof(Message));
            if (!contratAssemblies.Contains(pbbAsm))
                contratAssemblies.Add(pbbAsm);
            var typesFromAssemblies = assemblies.AsEnumerable()
                                                .SelectMany(assembly => assembly.GetTypes()
                                                                                .Where(x => x.GetCustomAttributes(false)
                                                                                             .Where(y => y.GetType().Name == "DataContractAttribute" || y.GetType().Name == "ProtoContractAttribute")
                                                                                             .Count() > 0))
                                                .ToList();
            typesFromAssemblies.AddRange(commonTypes);
            return typesFromAssemblies.ToArray();
        }

        /// <summary>
        /// Generates a valid contract ID which will be used by protobuf-net.
        /// </summary>
        /// <remarks>
        /// We need these special crafted IDs because of the protocol buffers specification defined here: https://developers.google.com/protocol-buffers/docs/proto
        /// and specifically "The smallest tag number you can specify is 1, and the largest is 229 - 1, or 536,870,911. You also cannot use the numbers 19000 though 19999"
        /// </remarks>
        /// <returns></returns>
        public static Guid GenerateContractId()
        { 
            var suggestedGuid = Guid.Empty;
            bool isValid = false;
            while (!isValid)
            {
                suggestedGuid = Guid.NewGuid();
                isValid = IsValidProtoregContractId(suggestedGuid);
            }
            return suggestedGuid;
        }

        public static bool IsValidProtobufField(int fieldNumber)
        {
            bool isValid = fieldNumber > 0 && (fieldNumber < 19000 || fieldNumber > 19999) && fieldNumber < 536870911;
            return isValid;
        }

        public static bool IsValidProtoregContractId(Guid contractId)
        {
            int contractHashCode = contractId.GetHashCode();
            int fieldNumber = Math.Abs(contractHashCode) / 4;
            bool isValid = IsValidProtobufField(fieldNumber) && Math.Abs(contractHashCode) % 4 == 0;
            return isValid;
        }
    }
}
