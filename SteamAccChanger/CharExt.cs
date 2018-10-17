using System.Text.RegularExpressions;

namespace SteamAccChanger
{
    internal static class CharExt
    {
        private static readonly Regex AlphaNumericRegex = new Regex(@"^[a-zA-Z0-9\s,]*$");

        public static bool IsAlphaNumeric(this char c)
        {
            return AlphaNumericRegex.IsMatch(c.ToString());
        }
    }
}
