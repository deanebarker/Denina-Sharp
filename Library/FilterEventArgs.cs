using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeninaSharp.Core
{
    public class FilterEventArgs
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public PipelineCommand Command { get; set; }

        public FilterEventArgs(PipelineCommand command, string input, string output)
        {
            Command = command;
            Input = input;
            Output = output;
        }
    }
}
