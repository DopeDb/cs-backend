using DopeDb.Shared.Caching.Backend;

namespace DopeDb.Shared.Caching.Frontend
{
    public class StringFrontend : AbstractCacheFrontend
    {
        public StringFrontend(string identifier, ICacheBackend backend) : base(identifier, backend)
        {
        }

        new public string Get(string key)
        {
            return this.backend.Get(key).ToString();
        }

        public void Set(string key, string value)
        {
            this.backend.Set(key, value);
        }
    }
}