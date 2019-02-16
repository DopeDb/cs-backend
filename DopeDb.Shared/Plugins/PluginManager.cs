using System.Reflection;

namespace DopeDb.Shared.Plugins
{
    public class PluginManager
    {
        protected Plugin[] plugins;

        public PluginManager()
        {
            this.plugins = new Plugin[]{new Plugin(Assembly.GetEntryAssembly())};
        }

        public Plugin[] GetAllPlugins()
        {
            return this.plugins;
        }

        public bool IsActive(Plugin plugin)
        {
            return true;
        }

        public void ActivatePlugin(Plugin plugin)
        {
            throw new System.NotImplementedException();
        }

        public void DeactivatePlugin(Plugin plugin)
        {
            throw new System.NotImplementedException();
        }

        public Plugin GetPlugin(string name)
        {
            foreach (var plugin in this.plugins)
            {
                if (plugin.GetName() == name)
                {
                    return plugin;
                }
            }
            return null;
        }
    }
}