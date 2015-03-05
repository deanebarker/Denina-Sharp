using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace DeninaSharp.Core
{
    public class PipelineVariable
    {
        public string Name { get; private set; }
        public object Value { get; private set; }
        public bool ReadOnly { get; private set; }

        public PipelineVariable(string name, object value, bool readOnly = false)
        {
            Name = name;
            Value = value;
            ReadOnly = readOnly;
        }

        public override string ToString()
        {
            return Value == null ? String.Empty : Value.ToString();
        }
    }
}
