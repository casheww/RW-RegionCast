
namespace RegionCast
{
    class Utils
    {
        public static string GetSlugName(string rawName)
        {
            switch (rawName)
            {
                case "Yellow":
                    return "Monk";

                case "White":
                    return "Survivor";

                case "Red":
                    return "Hunter";

                default:
                    return rawName;
            }
        }
    }
}
