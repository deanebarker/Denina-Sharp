using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace DeninaSharp.Core.Documentation
{
    public class CodeSampleDoc
    {
        private const string RESOURCE_PREFIX = "resource:";

        private string input;
        public string Input
        {
            get
            {
                return input;
            }
            set
            {
                input = Eval(value);
            }
        }

        private string command;
        public string Command
        {
            get
            {
                return command;
            }
            set
            {
                command = Eval(value);
            }
        }

        private string output;
        public string Output
        {
            get
            {
                return output;
            }
            set
            {
                output = Eval(value);
            }
        }


        private string Eval(string value)
        {
            if (!value.StartsWith(RESOURCE_PREFIX))
            {
                return value;
            }

            // Get rid of the prefix
            value = value.Substring(RESOURCE_PREFIX.Length);

            var assembly = Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();
            if(!resources.Contains(value))
            {
                // No exact match, let's try to match on ending...
                var possibleMatch = resources.Where(s => s.EndsWith(value)).FirstOrDefault();
                if(possibleMatch != null)
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