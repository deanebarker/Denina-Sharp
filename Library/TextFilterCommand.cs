using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BlendInteractive.TextFilterPipeline.Core
{
    public class TextFilterCommand
    {
        public TextFilterPipeline Pipeline;
        public string CommandName { get; set; }
        public Dictionary<object, string> CommandArgs { get; set; }
        public string VariableName { get; set; }

        public TextFilterCommand()
        {
            CommandArgs = new Dictionary<object, string>();
        }       

        public string DefaultArgument
        {
            get { return CommandArgs.FirstOrDefault().Value; }
        }

        public string NormalizedCommandName
        {
            get { return CommandName.ToLower(); }
        }

        public void ResolveArguments()
        {
            // This is a whole lot of crap to get around the problem of modifying a collection while it's being iterated...

            var variablesToResolve = new Dictionary<object, string>();
            foreach (var commandArg in CommandArgs)
            {
                if (CommandParser.IsVariableName(commandArg.Value))
                {
                    variablesToResolve.Add(commandArg.Key, commandArg.Value);
                }
            }

            foreach (var variable in variablesToResolve)
            {
                CommandArgs[variable.Key] = Pipeline.GetVariable(CommandParser.StripVariablePrefix(variable.Value)).ToString();
            }
        }
    }
}