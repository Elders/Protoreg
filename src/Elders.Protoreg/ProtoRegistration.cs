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
    }
}
