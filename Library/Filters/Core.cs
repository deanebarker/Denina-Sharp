using System;
using System.Linq;
using System.Web.UI;
using BlendInteractive.Denina.Core.Documentation;
using DeninaSharp.Core.Documentation;

namespace DeninaSharp.Core.Filters
{
    [Filters("Core", "For working with the pipeline and its variables.")]
    public static class Core
    {
        [Filter("Clear", "Replaces the input text. (The same as ReplaceAll called with no arguments.)")]
        [CodeSample("(The contents of War and Peace...)", "Clear", "(Absolutely nothing.)")]
        public static string Clear(string input, PipelineCommand command)
        {
            return String.Empty;
        }

        [Filter("ReadFrom", "Sets the active text to the contents of a variable.")]
        [ArgumentMeta(1, "Variable Name", true, "The name of the variable to be retrieved.")]
        [CodeSample("", "SetVar Name \"James Bond\"\nReadFrom Name", "James Bond")]
        [DoNotResolveVariables]
        public static string ReadFrom(string input, PipelineCommand command)
        {
            // This is a placeholder. No code will ever get here. See the "Execute" method of TextFilterPipeline.
            return String.Empty;
        }

        [Filter("WriteTo", "Writes the active text to the named variable.")]
        [ArgumentMeta(1, "Variable Name", true, "The name of the variable to which to write the input string.")]
        [CodeSample("James Bond", "WriteTo Name", "(The variable \"Name\" now contains \"James Bond\".)")]
        [DoNotResolveVariables]
        public static string WriteTo(string input, PipelineCommand command)
        {
            // This is a placeholder. No code will ever get here. See the "Execute" method of TextFilterPipeline.
            return String.Empty;
        }

        [Filter("SetVar", "Sets the value of a variable to the value provided. Does not change the input string.")]
        [ArgumentMeta(1, "Variable Name", true, "The name of the variable to set.")]
        [ArgumentMeta(2, "Value", false, "The desired value. If not provided, the variable is set to an empty string (same as InitVar).")]
        [CodeSample("", "SetVar Name \"James Bond\"\nReadFrom Name", "James Bond")]
        [DoNotResolveVariables]
        public static string SetVar(string input, PipelineCommand command)
        {
            var variableName = command.GetArgument("var");
            var value = command.GetArgument("value", String.Empty);
            
            command.Pipeline.SafeSetVariable(variableName, value);

            return input;
        }

        [Filter("InitVar", "Sets the value of a variable to an empty string. The variable can now be referenced without error.")]
        [ArgumentMeta(1, "Variable Name", true, "The name of the variable to set. Multiple variables can be specified. All will be initialized.")]
        [DoNotResolveVariables]
        [CodeSample("", "InitVar Name Address City State Zip", "(None. The named variables are all initialized to empty strings.)")]
        public static string InitVar(string input, PipelineCommand command)
        {
            foreach (var variableName in command.GetMultiArgument("var"))
            {
                command.Pipeline.SafeSetVariable(variableName, String.Empty);
            }
            return input;
        }

        [Filter("Now", "Returns the current date and time, formatted by an optional format string.")]
        [ArgumentMeta(1, "Format String", false, "The C# time format string with which to format the results.")]
        [CodeSample("", "Now \"ddd d MMM\"", "Wed 25 Feb")]
        public static string Now(string input, PipelineCommand command)
        {
            var formatString = "f";
            if (command.CommandArgs.Count == 1)
            {
                formatString = command.GetArgument(0);
            }
            return DateTime.Now.ToString(formatString);
        }

        [Filter("AppendVar", "Appends to the value of a variable")]
        [ArgumentMeta(1, "Variable Name", true, "The name of the variable to which to append data.")]
        [ArgumentMeta(2, "Data", false, "The value to append. If omitted, the input string will be appended.")]
        [CodeSample("", "SetVar Name James\nAppendVar Name \" Bond\"\nReadFrom Name", "James Bond")]
        public static string AppendVar(string input, PipelineCommand command)
        {
            // If they didn't provide a value, then use the input
            var valueToAppend = command.GetArgument("value", input);
            var variableName = command.GetArgument("var");

            var oldValue = command.Pipeline.GetVariable(variableName);
            var newValue = String.Concat(oldValue, valueToAppend);

            command.Pipeline.SafeSetVariable(variableName, newValue);

            return input;
        }

        [Filter("Label", "A placeholder filter for a label. Simply passes text through.")]
        public static string Pass(string input, PipelineCommand command)
        {
            return input;
        }

        [Filter("End", "Ends execution.")]
        public static string End(string input, PipelineCommand command)
        {
            command.SendToLabel = "end";
            return input;
        }


    }
}