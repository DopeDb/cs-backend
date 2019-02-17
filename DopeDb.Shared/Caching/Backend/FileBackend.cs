using DopeDb.Shared.Util;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System;

namespace DopeDb.Shared.Caching.Backend
{
    public class FileBackend : ICacheBackend
    {
        protected string identifier;

        protected string baseDir;

        protected int nestingDepth;

        protected int timeToLive;

        public FileBackend(string identifier, int nestingDepth = 1, int timeToLive = 0)
        : this(identifier, Shared.Util.Path.CacheDir(), nestingDepth, timeToLive)
        {
        }

        public FileBackend(string identifier, string baseDir, int nestingDepth = 1, int timeToLive = 0)
        {
            if (Regex.IsMatch(identifier, @"[^a-zA-Z0-9._-]"))
            {
                throw new System.ArgumentException($"Cache identifier may only contain alpha-numeric characters, - _ and ., given: {identifier}");
            }
            this.identifier = identifier;
            this.baseDir = baseDir;
            this.nestingDepth = nestingDepth;
            this.timeToLive = timeToLive;
        }

        public string Get(string key)
        {
            return Has(key) ? File.ReadAllText(GetFileName(key)) : string.Empty;
        }

        public bool Has(string key)
        {
            var filePath = GetFileName(key);
            if (!File.Exists(filePath))
                return false;
            if (this.timeToLive == 0)
                return true;
            var threshold = DateTime.Now.AddSeconds(-this.timeToLive);
            return File.GetLastWriteTime(filePath) > threshold;
        }

        public void Remove(string key)
        {
            File.Delete(GetFileName(key));
        }

        public void Set(string key, string value)
        {
            var path = GetFileName(key);
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            File.WriteAllText(path, value);
        }

        protected string GetFileName(string key)
        {
            var sBuilder = new StringBuilder();
            sBuilder.Append(this.baseDir);
            sBuilder.Append(System.IO.Path.DirectorySeparatorChar);
            sBuilder.Append(this.identifier);
            sBuilder.Append(System.IO.Path.DirectorySeparatorChar);
            var nameHash = Algorithm.GetMd5(key);
            string[] nameParts = new string[this.nestingDepth + 1];
            for (int i = 0; i < this.nestingDepth; i++)
            {
                sBuilder.Append(nameHash[i]);
                sBuilder.Append(System.IO.Path.DirectorySeparatorChar);
            }
            sBuilder.Append(nameHash.Substring(this.nestingDepth));
            return sBuilder.ToString();
        }
    }
}