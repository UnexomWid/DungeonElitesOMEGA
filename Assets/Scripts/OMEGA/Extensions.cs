using System.Globalization;

namespace OMEGA
{
    public static class Extensions
    {
        public static string ToPrettyString(this Data.TrialMode val)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(val.ToString().ToLowerInvariant());
        }
    }
}