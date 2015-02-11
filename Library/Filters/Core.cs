using System;
using System.Linq;
using BlendInteractive.Denina.Core.Documentation;

namespace BlendInteractive.Denina.Core.Filters
{
    [Filters("Core", "For working with the pipeline and its variables.")]
    public static class Core
    {
        [Filter("Clear", "Replaces the input text. (The same as ReplaceAll called with no arguments.)")]
        public static string Clear(string input, PipelineCommand command)
        {
            return String.Empty;
        }

        [Filter("ReadFrom", "Sets the active text to the contents of a variable.")]
        [ArgumentMeta(1, "Variable Name", true, "The name of the variable to be retrieved.")]
        [DoNotResolveVariables]
        public static string ReadFrom(string input, PipelineCommand command)
        {
            // This is a placeholder. No code will ever get here. See the "Execute" method of TextFilterPipeline.
            return String.Empty;
        }

        [Filter("WriteTo", "Writes the active text to the named variable.")]
        [ArgumentMeta(1, "Variable Name", true, "The name of the variable to which to write the input string.")]
        [DoNotResolveVariables]
        public static string WriteTo(string input, PipelineCommand command)
        {
            // This is a placeholder. No code will ever get here. See the "Execute" method of TextFilterPipeline.
            return String.Empty;
        }

        [Filter("SetVar", "Sets the value of a variable to the value provided. Does not change the input string.")]
        [ArgumentMeta(1, "Variable Name", true, "The name of the variable to set.")]
        [ArgumentMeta(2, "Value", false, "The desired value. If not provided, the variable is set to an empty string (same as InitVar).")]
        [DoNotResolveVariables]
        public static string SetVar(string input, PipelineCommand command)
        {
            string value = String.Empty;
            if (command.CommandArgs.Count > 1)
            {
                value = command.CommandArgs[1];
            }

            command.Pipeline.SafeSetVariable(command.CommandArgs.First().Value, value);

            return input;
        }

        [Filter("InitVar", "Sets the value of a variable to an empty string. The variable can now be referenced without error.")]
        [ArgumentMeta(1, "Variable Name", true, "The name of the variable to set. Multiple variables can be specified. All will be initialized.")]
        [DoNotResolveVariables]
        public static string InitVar(string input, PipelineCommand command)
        {
            foreach (var commandArg in command.CommandArgs)
            {
                command.Pipeline.SafeSetVariable(commandArg.Value, String.Empty);
            }
            return input;
        }

        [Filter("Now", "Returns the current date and time, formatted by an optional format string.")]
        [ArgumentMeta(1, "Format String", false, "The C# time format string with which to format the results.")]
        public static string Now(string input, PipelineCommand command)
        {
            var formatString = "f";
            if (command.CommandArgs.Count == 1)
            {
                formatString = command.CommandArgs.First().ToString();
            }
            return DateTime.Now.ToString(formatString);
        }
    }
}