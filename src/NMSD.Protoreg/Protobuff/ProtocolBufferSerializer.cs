using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using ProtoBuf;
using ProtoBuf.Meta;

namespace NMSD.Protoreg.Protobuff
{
    public class ProtocolBufferSerializer
    {
        private const int GuidLengthInBytes = 16;

        private static readonly MD5 TypeHasher = MD5.Create(); // creates 16-byte hashes

        private readonly Dictionary<Type, ObjectDeserializer> deserializers = new Dictionary<Type, ObjectDeserializer>();

        private readonly Dictionary<Guid, Type> hashes = new Dictionary<Guid, Type>();

        Dictionary<long, Type> runtimeModelFieldNumbers = new Dictionary<long, Type>();

        ConcurrentDictionary<Type, Type> stronglyTypedWrappers = new ConcurrentDictionary<Type, Type>();

        private readonly Dictionary<Type, Guid> types = new Dictionary<Type, Guid>();

        public delegate object ObjectDeserializer(params object[] args);

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
                FastActivator.WarmInstanceConstructor(contract);
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

        public virtual object Deserialize(Stream input)
        {
            if (input == null)
                return null;

            Type contractType = ReadContractType(input);
            return deserializers[contractType](input);
        }

        public T Deserialize<T>(Stream input)
        {
            return (T)Deserialize(input);
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

            var message = (Message)graph;
            var wrapped = (Message)FastActivator.CreateInstance(BuildSerializationWrapperType(message.Body.GetType()));
            wrapped.Body = message.Body;
            Serializer.Serialize(output, wrapped);
        }

        private Type BuildSerializationWrapperType(Type contract)
        {
            Type stronglyTypedWrapper;
            if (!stronglyTypedWrappers.TryGetValue(contract, out stronglyTypedWrapper))
            {
                var wrapperType = typeof(StronglyTypedMessage<>);
                Type[] typeArgs = { contract };
                stronglyTypedWrapper = wrapperType.MakeGenericType(typeArgs);
                stronglyTypedWrappers.TryAdd(contract, stronglyTypedWrapper);
            }
            return stronglyTypedWrapper;
        }

        private bool CanRegisterContract(Type contract)
        {
            return contract != null
                && !types.ContainsKey(contract)
                && !string.IsNullOrEmpty(contract.FullName);
        }

        private bool CanRegisterContractToRuntimeModel(Type contract)
        {
            return !(typeof(System.Collections.IEnumerable).IsAssignableFrom(contract) || (typeof(string) == contract));
        }

        static ObjectDeserializer GetDeserializer(MethodInfo ctor)
        {
            ParameterInfo[] paramsInfo = ctor.GetParameters();
            ParameterExpression param = Expression.Parameter(typeof(object[]), "args");
            Expression[] argsExp = new Expression[paramsInfo.Length];
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;
                Expression paramAccessorExp = Expression.ArrayIndex(param, index);
                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);
                argsExp[i] = paramCastExp;
            }
            MethodCallExpression newExp = Expression.Call(ctor, argsExp);
            LambdaExpression lambda = Expression.Lambda(typeof(ObjectDeserializer), newExp, param);
            return (ObjectDeserializer)lambda.Compile();
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

        private void RegisterCommonTypes()
        {
            RegisterContract(typeof(Dictionary<string, object>));
            RegisterContract(typeof(Message), typeof(StronglyTypedMessage<dynamic>));
            RegisterContract(typeof(Exception));
            RegisterContract(typeof(SerializationException));
        }

        private void RegisterContract(Type contract)
        {
            RegisterContract(new Tuple<Type, Type>(typeof(object), contract), contract);
        }

        private void RegisterContract(Tuple<Type, Type> contract)
        {
            RegisterContract(contract, contract.Item2);
        }

        private void RegisterContract(Tuple<Type, Type> contract, Type serializationDtoContract)
        {
            if (!CanRegisterContract(contract.Item2))
                return;

            RegisterHash(contract.Item2);
            RegisterDeserializer(contract.Item2, serializationDtoContract);
            RegisterToRuntimeModel(contract);
        }

        private void RegisterContract(Type contract, Type serializationDtoContract)
        {
            RegisterContract(new Tuple<Type, Type>(typeof(object), contract), serializationDtoContract);
        }

        private void RegisterDeserializer(Type contract, Type serializationDtoContract)
        {
            var deserialize = typeof(Serializer).GetMethod("Deserialize").MakeGenericMethod(serializationDtoContract);
            deserializers[contract] = GetDeserializer(deserialize);
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