namespace DopeDb.Shared.Database
{
    public class PersistenceException : System.Exception
    {
        public PersistenceException() : base() { }
        public PersistenceException(string message) : base(message) { }
        public PersistenceException(string message, System.Exception e) : base(message, e) { }
    }
}