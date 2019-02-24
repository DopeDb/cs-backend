namespace DopeDb.Shared.Database.QueryBuilder
{
    public class Or : IConstraint
    {

        public IConstraint[] Parts { get; }

        public Or(params IConstraint[] conditions)
        {
            this.Parts = conditions;
        }
    }
}