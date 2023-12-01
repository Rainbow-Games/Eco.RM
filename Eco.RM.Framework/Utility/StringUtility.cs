namespace Eco.RM.Framework.Utility
{
    public static class StringUtility
    {
        public static string GetAssemblyNameFromAssemblyString(string qualifiedname)
        {
            var splits = qualifiedname.Split(",", StringSplitOptions.TrimEntries);
            return splits[1];
        }
    }
}
