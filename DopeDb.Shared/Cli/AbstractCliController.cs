namespace DopeDb.Shared.Cli
{

    public abstract class AbstractCliController
    {
        protected CliRequest request = null;

        public void SetCliReqest(CliRequest request)
        {
            this.request = request;
        }
    }
}