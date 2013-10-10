using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using ProtoBuf.Meta;

namespace LaCore.Hyperion.Infrastructure.MassTransit.Protobuff
{
    public class ProtocolBufferSerializer
    {
        private const int GuidLengthInBytes = 16;

        private static readonly MD5 TypeHasher = MD5.Create(); // creates 16-byte hashes

        private readonly Dictionary<Type, Func<Stream, object>> deserializers = new Dictionary<Type, Func<Stream, object>>();

        private readonly Dictionary<Guid, Type> hashes = new Dictionary<Guid, Type>();

        Dictionary<long, Type> runtimeModelFieldNumbers = new Dictionary<long, Type>();

        private readonly Dictionary<Type, Guid> types = new Dictionary<Type, Guid>();

        private ProtocolBufferSerializer()
        {
            RegisterCommonTypes();
        }

        public ProtocolBufferSerializer(params Type[] dataContracts)
            : this()
        {
            var contracts = dataContracts ?? new Type[] { };
            foreach (var contract in contracts)
            {
                if (contracts.Contains(contract.BaseType))
                    RegisterContract(new Tuple<Type, Type>(contract.BaseType, contract));
                else
                    RegisterContract(new Tuple<Type, Type>(typeof(object), contract));
            }
        }

        public ProtocolBufferSerializer(params Assembly[] contractAssemblies)
            : this(contractAssemblies.SelectMany(assembly => assembly.GetTypes().Where(x => x.GetCustomAttributes(false).Where(y => y.GetType().Name == "DataContractAttribute" || y.GetType().Name == "ProtoContractAttribute").Count() > 0)).ToArray())
        {
        }

        public T Deserialize<T>(Stream input)
        {
            return (T)Deserialize(input);
        }

        public virtual object Deserialize(Stream input)
        {
            if (input == null)
                return null;

            var contractType = ReadContractType(input);
            var deserialized = deserializers[contractType](input);
            if (deserialized is List<StronglyTypedMessage<dynamic>>)
                return ((List<StronglyTypedMessage<dynamic>>)deserialized).ConvertAll(x => x.ToEventMessage()).ToList();
            else
                return deserialized;
        }

        public void Serialize<T>(Stream output, T graph)
        {
            Serialize(output, (object)graph);
        }

        public virtual void Serialize(Stream output, object graph)
        {
            if (null == graph)
                return;

            var contract = graph.GetType();
            WriteContractTypeToStream(output, contract);

            if (graph is List<Message>)
            {
                var graphDto = ((List<Message>)graph).Select(x => (Message)Activator.CreateInstance(BuildSerializationWrapperType(x.Body.GetType()), new object[] { x.Headers, x.Body })).ToList();
                Serializer.Serialize(output, graphDto);
            }
            else
                Serializer.Serialize(output, graph);
        }

        private Type BuildSerializationWrapperType(Type contract)
        {
            var wrapperType = typeof(StronglyTypedMessage<>);
            Type[] typeArgs = { contract };
            return wrapperType.MakeGenericType(typeArgs);
        }

        private bool CanRegisterContract(Type contract)
        {
            return contract != null
                && !types.ContainsKey(contract)
                && !string.IsNullOrEmpty(contract.FullName);
        }

        private bool CanRegisterContractToRuntimeModel(Type contract)
        {
            return !(typeof(System.Collections.IEnumerable).IsAssignableFrom(contract)
                || (typeof(string) == contract));
        }

        private Type ReadContractType(Stream serialized)
        {
            var header = new byte[GuidLengthInBytes];
            serialized.Read(header, 0, header.Length);
            var hash = new Guid(header);

            Type contract;
            if (!hashes.TryGetValue(hash, out contract))
                throw new SerializationException("Unable to serialize" + hash.ToString());

            return contract;
        }

        public void RegisterCommonType(Type type)
        {
            commonTypes.Add(type);
        }

        private List<Type> commonTypes = new List<Type>();

        private void RegisterCommonTypes()
        {
            RegisterContract(typeof(Dictionary<string, object>));
            RegisterContract(typeof(List<Message>), typeof(List<StronglyTypedMessage<dynamic>>));
            RegisterContract(typeof(Exception));
            RegisterContract(typeof(SerializationException));
        }

        private void RegisterContract(Tuple<Type, Type> contract, Type serializationDtoContract)
        {
            if (!CanRegisterContract(contract.Item2))
                return;

            RegisterHash(contract.Item2);
            RegisterDeserializer(contract.Item2, serializationDtoContract);
            RegisterToRuntimeModel(contract);
        }

        private void RegisterContract(Type contract)
        {
            RegisterContract(new Tuple<Type, Type>(typeof(object), contract), contract);
        }

        private void RegisterContract(Tuple<Type, Type> contract)
        {
            RegisterContract(contract, contract.Item2);
        }

        private void RegisterContract(Type contract, Type serializationDtoContract)
        {
            RegisterContract(new Tuple<Type, Type>(typeof(object), contract), serializationDtoContract);
        }

        private void RegisterDeserializer(Type contract, Type serializationDtoContract)
        {
            // TODO: make this faster by using reflection to create a delegate and then invoking the delegate
            var deserialize = typeof(Serializer).GetMethod("Deserialize").MakeGenericMethod(serializationDtoContract);
            deserializers[contract] = stream => deserialize.Invoke(null, new object[] { stream });
        }

        private void RegisterHash(Type contract)
        {
            DataContractAttribute attribute = (DataContractAttribute)contract
                                                .GetCustomAttributes(false)
                                                .Where(attrib => attrib.GetType() == typeof(DataContractAttribute))
                                                .SingleOrDefault();

            Guid contractHash = Guid.Empty;
            if (attribute == null || String.IsNullOrWhiteSpace(attribute.Name))
            {
                var bytes = Encoding.Unicode.GetBytes(contract.FullName);
                contractHash = new Guid(TypeHasher.ComputeHash(bytes));
            }
            else
            {
                Guid contarctEmbededHash;
                if (Guid.TryParse(attribute.Name, out contarctEmbededHash))
                    contractHash = contarctEmbededHash;
            }

            if (contractHash == Guid.Empty)
                throw new Exception("Cannot register hash for contract " + contract.FullName);

            hashes[contractHash] = contract;
            types[contract] = contractHash;
        }

        private void RegisterToRuntimeModel(Tuple<Type, Type> contract)
        {
            if (CanRegisterContractToRuntimeModel(contract.Item2))
            {
                var hash = types[contract.Item2];

                int fieldNumber = Math.Abs(hash.GetHashCode()) / 4;
                bool shouldRegisterSubType = RuntimeTypeModel.Default[contract.Item1].GetSubtypes() == null || !RuntimeTypeModel.Default[contract.Item1].GetSubtypes().Any(x => x.FieldNumber == fieldNumber);
                if (shouldRegisterSubType)
                    RuntimeTypeModel.Default[contract.Item1].AddSubType(fieldNumber, contract.Item2);
                try { runtimeModelFieldNumbers.Add(fieldNumber, contract.Item2); }
                catch (ArgumentException)
                {
                    throw new Exception(String.Format("A duplicate runtime model field number detected for contract '{0}'. If you use DataContractAttribute with Name='some Guid' it is recommended to test the Guid with the following condition which must be TRUE: 'Math.Abs(theGuid.GetHashCode()) % 4 == 0'.", contract.Item2.FullName));
                }
            }
        }

        private void WriteContractTypeToStream(Stream output, Type contract)
        {
            Guid hash;
            if (!types.TryGetValue(contract, out hash))
                throw new SerializationException("Can not serialize:" + contract.FullName);

            var header = hash.ToByteArray();
            output.Write(header, 0, GuidLengthInBytes);
        }

    }
}
