using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeninaSharp.Core
{
    public class VariableEventArgs
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public Pipeline Pipeline { get; set; }

        public VariableEventArgs(Pipeline pipeline, string key, object value = null)
        {
            Key = key;
            Value = value;
            Pipeline = pipeline;
        }
    }
}