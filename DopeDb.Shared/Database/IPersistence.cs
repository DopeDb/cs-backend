using DopeDb.Shared.Database.QueryBuilder;

namespace DopeDb.Shared.Database
{
    public interface IPersistence
    {
        Migration CreateMigration();

        void ApplyMigration(Migration migration);

        object GetEntity(Query query);

        void AddEntity(object entity);

        void UpdateEntity(object entity);

        void DeleteEntity(object entity);
    }
}