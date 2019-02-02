using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace DopeDb.Shared.Reflection {
    public class Reflection {

        private static readonly Reflection instance = new Reflection();

        public static Reflection Instance()
        {
            return instance;
        }
        public System.Type[] GetSubclasses(System.Type t)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var referencedAssemblies = entryAssembly.GetReferencedAssemblies();
            var allAssemblies = new AssemblyName[referencedAssemblies.Length + 1];
            allAssemblies[0] = entryAssembly.GetName();
            for (int i = 0; i < referencedAssemblies.Length; i++)
            {
                allAssemblies[i + 1] = referencedAssemblies[i];
            }
            var subTypes = allAssemblies
                .Select(Assembly.Load)
                .SelectMany(x => x.DefinedTypes)
                .Where(type => t.GetTypeInfo().IsAssignableFrom(type.AsType())); 
            
            List<System.Type> result = new List<System.Type>();

            foreach (System.Type subType in subTypes)
            {
                if (subType.IsAbstract)
                {
                    continue;
                }
                result.Add(subType);
            }
            return result.ToArray();
        }
    }
}