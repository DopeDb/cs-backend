using DopeDb.Shared.Cli;

namespace DopeDb.Cli {
    class ServerCliController : AbstractCliController {
        public void StartCommand(int port = 8080)
        {
            System.Console.WriteLine(port);
        }
    }
}