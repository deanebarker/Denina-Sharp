using System;
using System.IO;
using System.Linq;
using BlendInteractive.TextFilterPipeline.Core.Documentation;

namespace BlendInteractive.TextFilterPipeline.Core.Filters
{
    [TextFilters("File", "Working with files on the file system.")]
    public class FileSystem
    {
        [TextFilter("Read", "Reads the content of a file on the file system.")]
        [ArgumentMeta(1, "Path", true, "The path to the file, relative to AppDomain.CurrentDomain.BaseDirectory. This value should not start with a leading slash as Path.Combine will interpret that as \"root.\"")]
        public static string Read(string input, TextFilterCommand command)
        {
            string fullPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                command.CommandArgs.First().Value.Replace("/", @"\\").TrimStart(@"\\".ToCharArray()));

            return File.ReadAllText(fullPath);
        }
    }
}