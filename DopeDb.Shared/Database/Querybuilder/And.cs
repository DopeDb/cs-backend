namespace DopeDb.Shared.Database.QueryBuilder
{
    public class And : IConstraint
    {

        public IConstraint[] Parts { get; }

        public And(params IConstraint[] conditions)
        {
            this.Parts = conditions;
        }
    }
}