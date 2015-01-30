using System;
using System.Linq;
using System.Text.RegularExpressions;
using BlendInteractive.TextFilterPipeline.Core.Documentation;

namespace BlendInteractive.TextFilterPipeline.Core.Filters
{
    [TextFilters("Core", "For working with the pipeline and its variables.")]
    public static class Core
    {

        [TextFilter("Clear", "Replaces the input text. (The same as ReplaceAll called with no arguments.)")]
        public static string Clear(string input, TextFilterCommand command)
        {
            return String.Empty;
        }

        [TextFilter("ReadFrom", "Sets the active text to the contents of a variable.")]
        [ArgumentMeta(1, "Variable Name", true, "The name of the variable to be retrieved.")]
        [DoNotResolveVariables]
        public static string ReadFrom(string input, TextFilterCommand command)
        {
            // This is a placeholder. No code will ever get here. See the "Execute" method of TextFilterPipeline.
            return String.Empty;
        }

        [TextFilter("WriteTo", "Writes the active text to the named variable.")]
        [ArgumentMeta(1, "Variable Name", true, "The name of the variable to which to write the input string.")]
        [DoNotResolveVariables]
        public static string WriteTo(string input, TextFilterCommand command)
        {
            // This is a placeholder. No code will ever get here. See the "Execute" method of TextFilterPipeline.
            return String.Empty;
        }

        [TextFilter("SetVar", "Sets the value of a variable to the value provided. Does not change the input string.")]
        [ArgumentMeta(1, "Variable Name", true, "The name of the variable to set.")]
        [ArgumentMeta(2, "Value", false, "The desired value. If not provided, the variable is set to an empty string (same as InitVar).")]
        [DoNotResolveVariables]
        public static string SetVar(string input, TextFilterCommand command)
        {
            var value = String.Empty;
            if (command.CommandArgs.Count > 1)
            {
                value = command.CommandArgs[1];
            }

            command.Pipeline.SetVariable(command.CommandArgs.First().Value, value);

            return input;
        }

        [TextFilter("InitVar", "Sets the value of a variable to an empty string. The variable can now be referenced without error.")]
        [ArgumentMeta(1, "Variable Name", true, "The name of the variable to set. Multiple variables can be specified. All will be initialized.")]
        [DoNotResolveVariables]
        public static string InitVar(string input, TextFilterCommand command)
        {
            foreach (var commandArg in command.CommandArgs)
            {
                command.Pipeline.SetVariable(commandArg.Value, String.Empty);
            }
            return input;
        }
    }
}