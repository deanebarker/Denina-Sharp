using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlendInteractive.TextFilterPipeline.Core.Filters
{
    [TextFilters("file")]
    public class FileSystem
    {
        [TextFilter("read")]
        public static string Read(string input, TextFilterCommand command)
        {
            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, command.CommandArgs.First().Value.Replace("/", @"\\").TrimStart(@"\\".ToCharArray()));

            return File.ReadAllText(fullPath);

        }
    }
}
