namespace DopeDb.Shared.Caching.Backend
{
    public interface ICacheBackend
    {
        bool Has(string key);

        string Get(string key);

        void Set(string key, string value);

        void Remove(string key);
    }
}