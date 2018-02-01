using BlendInteractive.Denina.Core;
using DeninaSharp.Core.Documentation;

namespace DeninaSharp.Core.Filters
{
    [Filters("Test", "Filters for unit testing. These will not be written to the documentation.")]
    public class Test
    {
        [Filter("FakeTest", "This is a fake test which requires a fake class, used to test dependency checking")]
        [Requires("SomeFakeClass", "This class doesn't exist.")]
        public static string FakeTest(string input, PipelineCommand command, ExecutionLog log)
        {
            return input;
        }

    }
}
