namespace DopeDb.Shared.Caching.Frontend
{
    public interface ICacheFrontend
    {
        bool Has(string key);

        object Get(string key);

        void Set(string key, object value);

        void Remove(string key);
    }
}