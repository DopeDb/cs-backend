namespace DopeDb.Shared.Core.ObjectManagement
{
    public class ObjectManagementException : System.Exception
    {
        public ObjectManagementException() : base() { }
        public ObjectManagementException(string message) : base(message) { }
        public ObjectManagementException(string message, System.Exception e) : base(message, e) { }
    }
}