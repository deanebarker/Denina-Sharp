using System;
using System.IO;
using System.Linq;
using BlendInteractive.Denina.Core.Documentation;

namespace BlendInteractive.Denina.Core.Filters
{
    [Filters("File", "Working with files on the file system.")]
    public class File
    {
        public static readonly string SANDBOX_VARIABLE_NAME = "__filesystemsandbox";

        [Filter("Read", "Reads the content of a file on the file system.")]
        [ArgumentMeta(1, "Path", true, "The path to the file, relative to AppDomain.CurrentDomain.BaseDirectory. This value should not start with a leading slash as Path.Combine will interpret that as \"root.\"")]
        public static string Read(string input, PipelineCommand command)
        {
            // The sandbox variable must be set...
            if (!Pipeline.IsSetGlobally(SANDBOX_VARIABLE_NAME))
            {
                throw new DeninaException("File system sandbox variable has not been defined.");
            }

            string fullPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                Pipeline.GetGlobalVariable(SANDBOX_VARIABLE_NAME).ToString(),
                command.CommandArgs.First().Value.Replace("/", @"\\").TrimStart(@"\\".ToCharArray()));

            if (!System.IO.File.Exists(fullPath))
            {
                throw new DeninaException(String.Format("Attempt to read non-existent file: \"{0}\"", command.CommandArgs.First().Value));
            }

            return System.IO.File.ReadAllText(fullPath);
        }


    }
}

namespace BlendInteractive.Denina.Core
{
    public partial class Pipeline
    {
        public static void SetFileSandbox(string value)
        {
            SetGlobalVariable(BlendInteractive.Denina.Core.Filters.File.SANDBOX_VARIABLE_NAME, value, true);
        }
    }
}