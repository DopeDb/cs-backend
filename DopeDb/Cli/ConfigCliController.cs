using DopeDb.Shared.Cli;
using DopeDb.Shared.Configuration;
using DopeDb.Shared.Plugins;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DopeDb.Cli
{
    class ConfigCliController : AbstractCliController
    {
        [Description("Show all configured settings")]
        [Parameter("type", Description = "Settings, Routes etc.")]
        [Parameter("path", Description = "The path through the nested configuration")]
        public void ShowCommand(string type, string path = "")
        {
            var pluginManager = new PluginManager();
            System.Enum.TryParse(type, out ConfigurationType configType);
            var configurationManager = new Shared.Configuration.ConfigurationManager(pluginManager);
            var config = configurationManager.GetConfiguration(configType, path);
            PrintConfiguration(config);
        }

        protected void PrintConfiguration(IConfiguration config, int depth = 0)
        {
            foreach (var child in config.GetChildren())
            {
                Util.Write($"{child.Key}: ", depth * 2);
                if (child.Value == null && child.GetChildren().GetEnumerator().MoveNext())
                {
                    Util.WriteLine();
                    PrintConfiguration(child, depth + 1);
                }
                else
                {
                    Util.Write((object)child.Value);
                }
            }
        }
    }
}