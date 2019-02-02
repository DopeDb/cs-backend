namespace DopeDb {
    class DopeDb {
        public static void Main(string[] args)
        {
            var cliHandler = new Cli.CliHandler();
            cliHandler.HandleCliCommand(args);
        }
    }
}