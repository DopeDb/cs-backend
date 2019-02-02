
namespace DopeDb.Shared.Cli {
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class Description : System.Attribute
    {
        string description;

        public Description(string description)
        {
            this.description = description;
        }

        public string GetDescription()
        {
            return this.description;
        }
    }
}