using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeninaSharp.Core
{
    public class PipelineEventArgs: EventArgs
    {
        public PipelineEventArgs(Pipeline pipeline, string value)
        {
            Value = value;
            Pipeline = pipeline;
        }

        public string Value { get; set; }
        public Pipeline Pipeline { get; set; }
    }
}
