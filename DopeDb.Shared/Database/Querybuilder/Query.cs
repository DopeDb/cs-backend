using System.Collections.Generic;

namespace DopeDb.Shared.Database.QueryBuilder
{
    public class Query
    {
        public int Offset { get; }

        public int Limit { get; }

        public IConstraint Constraint { get; }

        public Dictionary<string, SortingDirection> SortingDirections { get; }

        public string[] SelectedProperties { get; }

        public Query(string[] properties, IConstraint constraint = null, Dictionary<string, SortingDirection> orderBy = null, int Offset = 0, int Limit = 0)
        {
            this.SelectedProperties = properties;
            this.Offset = Offset;
            this.Limit = Limit;
            this.Constraint = constraint;
            this.SortingDirections = orderBy;
        }
    }
}