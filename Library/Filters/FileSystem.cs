using System;
using System.IO;
using System.Linq;
using BlendInteractive.Denina.Core.Documentation;

namespace BlendInteractive.Denina.Core.Filters
{
    [Filters("File", "Working with files on the file system.")]
    public class FileSystem
    {
        [Filter("Read", "Reads the content of a file on the file system.")]
        [ArgumentMeta(1, "Path", true, "The path to the file, relative to AppDomain.CurrentDomain.BaseDirectory. This value should not start with a leading slash as Path.Combine will interpret that as \"root.\"")]
        public static string Read(string input, PipelineCommand command)
        {
            string fullPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                command.CommandArgs.First().Value.Replace("/", @"\\").TrimStart(@"\\".ToCharArray()));

            return File.ReadAllText(fullPath);
        }
    }
}