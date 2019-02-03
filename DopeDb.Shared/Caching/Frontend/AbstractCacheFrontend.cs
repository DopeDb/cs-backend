using DopeDb.Shared.Caching.Backend;

namespace DopeDb.Shared.Caching.Frontend
{
    public abstract class AbstractCacheFrontend : ICacheFrontend
    {
        protected ICacheBackend backend;

        protected string identifier;

        public AbstractCacheFrontend(string identifier, ICacheBackend backend)
        {
            this.identifier = identifier;
            this.backend = backend;
        }
        public object Get(string key)
        {
            return this.backend.Get(key);
        }
        public bool Has(string key)
        {
            return this.backend.Has(key);
        }
        public void Set(string key, object value)
        {
            this.backend.Set(key, value.ToString());
        }

        public void Remove(string key)
        {
            this.backend.Remove(key);
        }
    }
}