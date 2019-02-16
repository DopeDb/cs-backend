using System.Reflection;

namespace DopeDb.Shared.Plugins
{
    public class Plugin
    {
        protected Assembly assembly;

        protected string name;
        public Plugin(Assembly assembly)
        {
            this.assembly = assembly;
            this.name = assembly.GetName().Name;
        }

        public Assembly GetAssembly()
        {
            return this.assembly;
        }

        public string GetName()
        {
            return this.name;
        }
    }
}