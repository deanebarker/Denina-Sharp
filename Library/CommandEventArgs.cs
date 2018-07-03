using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DeninaSharp.Core
{
    public class CommandEventArgs : EventArgs
    {
        public string FullyQualifiedCommandName { get; set; }
        public MethodInfo Method { get; set; }

        public bool Cancel { get; set; }

        public CommandEventArgs(string fullyQualifiedCommandName, MethodInfo method)
        {
            FullyQualifiedCommandName = fullyQualifiedCommandName;
            Method = method;
        }

    }
}
