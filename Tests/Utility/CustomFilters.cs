using BlendInteractive.Denina.Core;
using BlendInteractive.Denina.Core.Documentation;

namespace Tests.Utility
{
    [Filters("custom")]
    public static class CustomFilters
    {
        [Filter("MyMethod")]
        public static string MyMethod(string input, PipelineCommand command)
        {
            return "MyMethod";
        }
    }
}