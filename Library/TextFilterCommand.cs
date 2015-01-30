using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using BlendInteractive.TextFilterPipeline.Core.Documentation;

namespace BlendInteractive.TextFilterPipeline.Core
{
    [DebuggerDisplay("{NormalizedCommandName}")]
    public class TextFilterCommand
    {
        public const string DEFAULT_CATEGORY_NAME = "core";

        public TextFilterPipeline Pipeline;
        public string CommandName { get; set; }
        public Dictionary<object, string> CommandArgs { get; set; }
        public string OutputVariable { get; set; }
        public string InputVariable { get; set; }

        public TextFilterCommand()
        {
            CommandArgs = new Dictionary<object, string>();
            InputVariable = TextFilterPipeline.GLOBAL_VARIABLE_NAME;
            OutputVariable = TextFilterPipeline.GLOBAL_VARIABLE_NAME;
        }       

        public string DefaultArgument
        {
            get { return CommandArgs.FirstOrDefault().Value; }
        }

        public string NormalizedCommandName
        {
            get { return CommandName.Contains(".") ? CommandName.ToLower() :  EnsureCategoryName(CommandName).ToLower(); }
        }

        public void ResolveArguments()
        {
            var method = TextFilterPipeline.CommandMethods[NormalizedCommandName];
            if (method.GetCustomAttributes(typeof (DoNotResolveVariablesAttribute), false).Any())
            {
                // We're not resolving attributes for this method. Bail out...
                return;
            }
            
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
                CommandArgs[variable.Key] = Pipeline.GetVariable(CommandParser.NormalizeVariableName(variable.Value)).ToString();
            }
        }

        public static string EnsureCategoryName(string input)
        {
            if (input.Contains("."))
            {
                return input;
            }
            return String.Concat(DEFAULT_CATEGORY_NAME, ".", input);
        }
    }
}