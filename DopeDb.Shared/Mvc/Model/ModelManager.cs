using DopeDb.Shared.Configuration;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace DopeDb.Shared.Mvc.Model
{
    public class ModelManager
    {
        protected ConfigurationManager configurationManager;

        public ModelManager(ConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public ModelDefinition.ModelDefinition BuildModelDefinition(string modelDefinitionIdentifier)
        {
            var modelConfiguration = this.configurationManager.GetConfiguration(ConfigurationType.Model, modelDefinitionIdentifier);
            if (modelConfiguration == null)
            {
                throw new System.Exception($"Could not find a model definition identified by {modelDefinitionIdentifier}");
            }
            var propertiesSection = modelConfiguration.GetSection("properties");
            if (!propertiesSection.GetChildren().GetEnumerator().MoveNext()) {
                throw new System.Exception($"No properties defined for model-definition {modelDefinitionIdentifier}");
            }
            var properties = new List<ModelDefinition.Property>();
            foreach (var propertyConfiguration in propertiesSection.GetChildren())
            {
                var propertyType = (string)ConfigurationManager.GetConfigurationValueByPath(propertyConfiguration, "type");
                if (propertyType == null)
                {
                    throw new System.ArgumentException($"The property \"{propertyConfiguration.Key}\" in {modelDefinitionIdentifier} has no type configured");
                }
                var uiConfiguration = ConfigurationManager.GetConfigurationByPath(propertyConfiguration, "ui").AsEnumerable();
                var propertyDefinition = new ModelDefinition.Property(propertyConfiguration.Key, uiConfiguration);
                propertyDefinition.Type = propertyType;
                properties.Add(propertyDefinition);
            }
            var modelDefinition = new ModelDefinition.ModelDefinition(
                modelDefinitionIdentifier,
                properties.ToArray()
            );
            return modelDefinition;
        }
    }
}