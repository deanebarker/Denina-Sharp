using BlendInteractive.TextFilterPipeline.Core;

namespace Tests.Utility
{
    [TextFilters("custom")]
    public static class CustomFilters
    {
        [TextFilter("MyMethod")]
        public static string MyMethod(string input, TextFilterCommand command)
        {
            return "MyMethod";
        }
    }
}