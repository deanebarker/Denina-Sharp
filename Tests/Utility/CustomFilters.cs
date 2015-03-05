using DeninaSharp.Core;
using DeninaSharp.Core.Documentation;

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