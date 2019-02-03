using DopeDb.Shared.Caching.Backend;

namespace DopeDb.Shared.Caching.Frontend
{
    public class StringFrontend : AbstractCacheFrontend
    {
        public StringFrontend(string identifier, ICacheBackend backend) : base(identifier, backend)
        {
        }

        public void Set(string key, string value)
        {
            this.backend.Set(key, value);
        }
    }
}