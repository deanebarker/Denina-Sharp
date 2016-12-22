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

        [Filter("MyMethodWithDependencies")]
        [Requires("SomeClassThatDoesntExist, SomeAssemblyThatDoesntExist", "")]
        public static string MyMethodWithDependencies(string input, PipelineCommand command)
        {
            // This shouldn't load, because it will fail the dependency check
            return "MyMethodWithDependencies";
        }
    }
}