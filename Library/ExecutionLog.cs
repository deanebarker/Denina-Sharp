using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DeninaSharp.Core
{
    public struct ExecutionLog
    {
        public string CommandName { get; private set; }
        public string CommandText { get; private set; }
        public string InputValue { get; set; }
        public string OutputValue { get; set; }
        public bool SuccessfullyExecuted { get; set; }

        public List<string> Messages { get; set; }

        public long ElapsedTime { get; set; }

        public Dictionary<string,string> Variables { get; set; }
        public List<DictionaryEntry> Arguments { get; set; }

        public ExecutionLog(PipelineCommand command, IDictionary<string, PipelineVariable> variables) : this()
        {
            CommandName = command.CommandName;
            CommandText = command.OriginalText;

            Variables = new Dictionary<string, string>();
            Arguments = new List<DictionaryEntry>();
            Messages = new List<string>();

            foreach(var argument in command.CommandArgs)
            {
                Arguments.Add(new DictionaryEntry(argument.Key.ToString(), argument.Value));
            }

            foreach(var variable in variables.Where(v => v.Key != Pipeline.GLOBAL_VARIABLE_NAME))
            {
                Variables.Add(variable.Key, variable.Value?.Value?.ToString());
            }
        }

        public void AddMessage(string message)
        {
            Messages.Add(message);
        }
    }
}
