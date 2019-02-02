namespace DopeDb.Shared.Util {
    public class Text {
        public static string FirstCharToUpper(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}