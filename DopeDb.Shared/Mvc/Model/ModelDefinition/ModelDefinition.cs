namespace DopeDb.Shared.Mvc.Model.ModelDefinition
{
    public class ModelDefinition
    {
        public string Identifier { get; }

        public Property[] Properties { get; }

        public string GetLabel()
        {
            return this.Identifier;
        }

        public ModelDefinition(string identifier, Property[] properties)
        {
            this.Identifier = identifier;
            this.Properties = properties;
        }
    }
}