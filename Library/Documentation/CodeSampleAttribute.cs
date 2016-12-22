using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DeninaSharp.Core.Documentation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [XmlRoot(ElementName = "codeSample")]
    public class CodeSampleAttribute : Attribute
    {
        public string Input { get; set; }
        public string Command { get; set; }
        public string Output { get; set; }

        public CodeSampleAttribute()
        {
            
        }

        public CodeSampleAttribute(string input, string command, string output)
        {
            Input = input;
            Command = command;
            Output = output;
        }
    }
}
