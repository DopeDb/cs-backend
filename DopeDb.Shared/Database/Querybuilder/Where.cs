namespace DopeDb.Shared.Database.QueryBuilder
{
    public class Where : IConstraint
    {
        public string Property { get; }

        public object Value { get; }

        public Where(string property, object value)
        {
            this.Property = property;
            this.Value = value;
        }
    }
}