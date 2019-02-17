using System.Collections.Generic;
using System.Linq;

namespace DopeDb.Shared.Mvc.Model.ModelDefinition
{
    public class Property
    {
        public string Type;

        public string Name { get; }

        protected Dictionary<string, string> UiConfiguration;

        public Property(string name, IEnumerable<KeyValuePair<string, string>> uiConfiguration)
        {
            this.Name = name;
            this.UiConfiguration = uiConfiguration.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public string UiConfigurationValue(string path)
        {
            path = path.Replace('.', ':');
            if (!this.UiConfiguration.ContainsKey(path))
            {
                return string.Empty;
            }
            return this.UiConfiguration[path];
        }
    }
}