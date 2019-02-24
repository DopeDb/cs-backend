namespace DopeDb.Shared.Database
{
    public enum MigrationDirection
    {
        Up,
        Down,
    }

    public class Migration
    {
        public int Version { get; }

        public string Platform { get; }

        protected object[] actionsUp;

        protected object[] actionsDown;

        public Migration(string platform, int version, object[] actionsUp, object[] actionsDown)
        {
            this.Platform = platform;
            this.Version = version;
            this.actionsUp = actionsUp;
            this.actionsDown = actionsDown;
        }

        public object[] GetActions(MigrationDirection direction)
        {
            switch (direction)
            {
                case MigrationDirection.Up:
                    return this.actionsUp;
                case MigrationDirection.Down:
                    return this.actionsDown;
            }
            return null;
        }
    }
}