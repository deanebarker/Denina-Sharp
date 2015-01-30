using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BlendInteractive.Denina.Core.Documentation;

namespace BlendInteractive.Denina.Core
{
    [DebuggerDisplay("{NormalizedCommandName}")]
    public class PipelineCommand
    {
        private const string DEFAULT_CATEGORY_NAME = "core";

        public Pipeline Pipeline;

        public PipelineCommand()
        {
            CommandArgs = new Dictionary<object, string>();
            InputVariable = Pipeline.GLOBAL_VARIABLE_NAME;
            OutputVariable = Pipeline.GLOBAL_VARIABLE_NAME;
        }

        public string CommandName { get; set; }
        public Dictionary<object, string> CommandArgs { get; set; }
        public string OutputVariable { get; set; }
        public string InputVariable { get; set; }

        public string DefaultArgument
        {
            get { return CommandArgs.FirstOrDefault().Value; }
        }

        public string NormalizedCommandName
        {
            get { return CommandName.Contains(".") ? CommandName.ToLower() : EnsureCategoryName(CommandName).ToLower(); }
        }

        public void ResolveArguments()
        {
            var method = Pipeline.CommandMethods[NormalizedCommandName];
            if (method.GetCustomAttributes(typeof (DoNotResolveVariablesAttribute), false).Any())
            {
                // We're not resolving attributes for this method. Bail out...
                return;
            }

            // This is a whole lot of crap to get around the problem of modifying a collection while it's being iterated...

            var variablesToResolve = new Dictionary<object, string>();
            foreach (var commandArg in CommandArgs)
            {
                if (PipelineCommandParser.IsVariableName(commandArg.Value))
                {
                    variablesToResolve.Add(commandArg.Key, commandArg.Value);
                }
            }

            foreach (var variable in variablesToResolve)
            {
                CommandArgs[variable.Key] = Pipeline.GetVariable(PipelineCommandParser.NormalizeVariableName(variable.Value)).ToString();
            }
        }

        public static string EnsureCategoryName(string input)
        {
            return input.Contains(".") ? input : String.Concat(DEFAULT_CATEGORY_NAME, ".", input);
        }
    }
}