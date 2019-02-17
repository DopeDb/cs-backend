using System.Reflection;

namespace DopeDb.Shared.Util
{
    public class Path
    {
        public static string CacheDir()
        {
            return GetEnvironmentVariable("DOPEDB_CACHEPATH", "../Cache");
        }

        public static string ConfigDir()
        {
            return GetEnvironmentVariable("DOPEDB_CONFIGPATH", "../Configuration");
        }

        protected static string GetEnvironmentVariable(string variable, string defaultValue)
        {
            var envVar = System.Environment.GetEnvironmentVariable(variable);
            if (envVar == null)
            {
                return defaultValue;
            }
            return envVar;
        }
    }
}