using System.Collections.Generic;

namespace DopeDb.Shared.Database.QueryBuilder
{
    public class QueryBuilder
    {

        protected string[] selectedProperties;

        protected IConstraint constraint;

        protected int offset = 0;

        protected int limit = -1;

        protected Dictionary<string, SortingDirection> orderBy;

        public QueryBuilder()
        {
            this.orderBy = new Dictionary<string, SortingDirection>();
        }

        public QueryBuilder Select(params string[] properties)
        {
            this.selectedProperties = properties;
            return this;
        }

        public QueryBuilder Offset(int offset)
        {
            this.offset = offset;
            return this;
        }

        public QueryBuilder Limit(int limit)
        {
            this.limit = limit;
            return this;
        }

        public QueryBuilder Filter(IConstraint filter)
        {
            this.constraint = filter;
            return this;
        }

        public And And(params IConstraint[] parts)
        {
            return new And(parts);
        }

        public Or Or(params IConstraint[] parts)
        {
            return new Or(parts);
        }

        public Where Where(string property, object value)
        {
            return new Where(property, value);
        }

        public QueryBuilder OrderBy(string property, SortingDirection direction = SortingDirection.Asc)
        {
            this.orderBy.Add(property, direction);
            return this;
        }

        public Query Build()
        {
            var query = new Query(selectedProperties, constraint, orderBy, offset, limit);
            return query;
        }

    }
}