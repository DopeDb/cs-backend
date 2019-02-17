using DopeDb.Shared.Cli;
using DopeDb.Shared.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DopeDb.Shared.Configuration
{

    public enum ConfigurationType
    {
        Settings,
        Routes,
        Model,
    }
    public class ConfigurationManager
    {
        protected Dictionary<ConfigurationType, IConfiguration> configurations;

        protected PluginManager pluginManager;

        public ConfigurationManager(PluginManager pluginManager)
        {
            this.pluginManager = pluginManager;
            this.loadConfiguration();
        }

        protected void loadConfiguration()
        {
            this.configurations = new Dictionary<ConfigurationType, IConfiguration>();
            var configurations = new Dictionary<ConfigurationType, ConfigurationBuilder>();
            foreach (var configType in (ConfigurationType[])Enum.GetValues(typeof(ConfigurationType)))
            {
                configurations.Add(configType, new ConfigurationBuilder());
            }
            var configTypes = String.Join("|", Enum.GetNames(typeof(ConfigurationType)));
            var resourceNameRegex = new Regex($@"({configTypes})\.(.+\.)*ya?ml");
            foreach (var plugin in this.pluginManager.GetAllPlugins())
            {
                if (!this.pluginManager.IsActive(plugin))
                {
                    continue;
                }
                var assembly = plugin.GetAssembly();
                var fileProvider = new EmbeddedFileProvider(assembly, "");
                var resources = assembly.GetManifestResourceNames();
                foreach (var resourceName in resources)
                {
                    var matches = resourceNameRegex.Matches(resourceName);
                    if (matches.Count == 0)
                    {
                        continue;
                    }
                    try
                    {
                        var configTypeName = matches[0].Groups[1].Value;
                        Enum.TryParse(configTypeName, out ConfigurationType configType);
                        var resourceStream = assembly.GetManifestResourceStream(resourceName);
                        configurations[configType].AddYamlFile(fileProvider, resourceName, false, false);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            var configDir = new DirectoryInfo(Shared.Util.Path.ConfigDir());
            if (configDir.Exists)
            {
                foreach (FileInfo file in configDir.GetFiles()) {
                    var matches = resourceNameRegex.Matches(file.Name);
                    if (matches.Count == 0)
                    {
                        continue;
                    }
                    var configTypeName = matches[0].Groups[1].Value;
                    Enum.TryParse(configTypeName, out ConfigurationType configType);
                    configurations[configType].AddYamlFile(file.FullName, true, false);
                }
            }
            foreach (var kv in configurations)
            {
                this.configurations.Add(kv.Key, kv.Value.Build());
            }
        }

        public IConfiguration GetConfiguration(ConfigurationType configurationType, IEnumerable<string> path)
        {
            if (this.configurations == null || !this.configurations.ContainsKey(configurationType))
            {
                throw new ArgumentOutOfRangeException("No configuration found for this configuration type");
            }
            var configuration = this.configurations[configurationType];
            if (path.Count() == 0)
            {
                return configuration;
            }
            var pathParts = path.ToArray();
            for (int i = 0; i < pathParts.Length; i++)
            {
                var key = pathParts[i];
                var currentChildren = configuration.GetChildren();
                var found = false;
                foreach (var child in currentChildren)
                {
                    if (child.Key == key)
                    {
                        configuration = child;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return null;
                }
            }
            return configuration;
        }

        public IConfiguration GetConfiguration(ConfigurationType configurationType, string path = "")
        {
            return GetConfiguration(configurationType, path.Length > 0 ? path.Split('.') : new string[] { });
        }

        public object GetConfigurationValue(ConfigurationType configurationType, string path)
        {
            var pathParts = path.Split('.');
            var lastKey = pathParts[pathParts.Length - 1];
            var config = this.GetConfiguration(configurationType, pathParts.SkipLast(1));
            if (config == null)
            {
                return null;
            }
            var section = config.GetSection(lastKey);
            return section.Value;
        }
    }
}