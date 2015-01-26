using System;
using System.IO;
using System.Linq;

namespace BlendInteractive.TextFilterPipeline.Core.Filters
{
    [TextFilters("File")]
    public class FileSystem
    {
        [TextFilter("Read")]
        public static string Read(string input, TextFilterCommand command)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, command.CommandArgs.First().Value.Replace("/", @"\\").TrimStart(@"\\".ToCharArray()));

            return File.ReadAllText(fullPath);
        }
    }
}