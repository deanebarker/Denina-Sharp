using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DeninaSharp.Core.Documentation
{
    public class BaseDoc
    {
        protected const string RESOURCE_PREFIX = "resource:";

        protected string Eval(string value)
        {
            if (!value.StartsWith(RESOURCE_PREFIX))
            {
                return value;
            }

            // Get rid of the prefix
            value = value.Substring(RESOURCE_PREFIX.Length);

            var assembly = Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();
            if (!resources.Contains(value))
            {
                // No exact match, let's try to match on ending...
                var possibleMatch = resources.Where(s => s.EndsWith(value)).FirstOrDefault();
                if (possibleMatch != null)
                {
                    value = possibleMatch;
                }
                else
                {
                    // No match, we're just going to send back the file name
                    return "Could not find embedded resource: " + value;
                }
            }

            using (var stream = assembly.GetManifestResourceStream(value))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
