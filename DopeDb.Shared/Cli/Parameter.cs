
namespace DopeDb.Shared.Cli {
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
    public class Parameter : System.Attribute
    {
        string name;
        public string Description = "";

        public Parameter(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return this.name;
        }
    }
}