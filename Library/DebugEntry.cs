using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeninaSharp.Core;

namespace BlendInteractive.Denina.Core
{
    public struct DebugEntry
    {
        public string CommandName { get; private set; }
        public string CommandText { get; private set; }
        public string InputValue { get; set; }
        public string OutputValue { get; set; }
        public bool SuccessfullyExecuted { get; set; }

        public long ElapsedTime { get; set; }

        public Dictionary<string,string> Variables { get; set; }
        public Dictionary<string,string> Arguments { get; set; }

        public DebugEntry(PipelineCommand command, IDictionary<string, PipelineVariable> variables) : this()
        {
            CommandName = command.CommandName;
            CommandText = command.OriginalText;

            Variables = new Dictionary<string, string>();
            Arguments = new Dictionary<string, string>();

            foreach(var argument in command.CommandArgs)
            {
                Arguments.Add(argument.Key.ToString(), argument.Value);
            }

            foreach(var variable in variables.Where(v => v.Key != Pipeline.GLOBAL_VARIABLE_NAME))
            {
                Variables.Add(variable.Key, variable.Value?.Value?.ToString());
            }
        }
    }
}
