using System;
using System.Linq;
using System.Text.RegularExpressions;
using BlendInteractive.TextFilterPipeline.Core.Documentation;

namespace BlendInteractive.TextFilterPipeline.Core.Filters
{
    [TextFilters("Core", "Basic string manipulation.")]
    public static class Core
    {
        [TextFilter("Append", "Appends a string to the input string.")]
        [ArgumentMeta(1, "Suffix", true, "The string to append to the input string." )]
        public static string Append(string input, TextFilterCommand command)
        {
            return String.Concat(input, command.DefaultArgument);
        }

        [TextFilter("Prepend", "Prepends a string to the input string.")]
        [ArgumentMeta(1, "Prefix", true, "The string to prepend to the input string.")]
        public static string Prepend(string input, TextFilterCommand command)
        {
            return String.Concat(command.DefaultArgument, input);
        }


        [TextFilter("Replace", "Finds and replaces one string with another.")]
        [ArgumentMeta(1, "Old String", true, "The string to be replaced.")]
        [ArgumentMeta(2, "New String", false, "The new string. If this argument is not provided, String.Empty will be used (so, the Old String will simply be removed.)")]
        public static string Replace(string input, TextFilterCommand command)
        {

            // If they didn't pass in a second argument, then we're using String.Empty (so, less "replace" and more "remove"
            var replaceWith = command.CommandArgs.Count > 1 ? command.CommandArgs[1] : String.Empty;

            return input.Replace(command.CommandArgs[0], replaceWith);
        }

        [TextFilter("ReplaceAll", "Completely replaces the input string.")]
        [ArgumentMeta(1, "New String", false, "The new string. After this filter executes, the active text will be set to this value. If no argument is provided, String.Empty is used, effectively clearing the active text." )]
        public static string ReplaceAll(string input, TextFilterCommand command)
        {
            if (!command.CommandArgs.Any())
            {
                return String.Empty;
            }

            return command.DefaultArgument;
        }

        [TextFilter("Format", "Performs simple token replacement of variables and the active text.")]
        [ArgumentMeta(1, "Format String", true, "A format string suitable for usage in String.Format. The input string will replace the {0} token. Variable values will replace {variableName} tokens.")]
        public static string Format(string input, TextFilterCommand command)
        {
            string template = command.CommandArgs.ContainsKey(0) ? command.CommandArgs[0] : String.Empty;
            template = command.CommandArgs.ContainsKey("template") && !String.IsNullOrWhiteSpace(command.CommandArgs["template"]) ? command.CommandArgs["template"] : template;

            foreach (var variable in command.Pipeline.Variables)
            {
                template = template.Replace(String.Concat("{", variable.Key, "}"), variable.Value.ToString());
            }

            // Escape any remaining {string} patterns, because these will break String.Format...
            template = Regex.Replace(template, "{([^0-9]+)}", "{{$1}}");

            return String.Format(template, input);
        }

        [TextFilter("Trim", "Trims whitespace from the ends of the input string.")]
        public static string Trim(string input, TextFilterCommand command)
        {
            return input.Trim();
        }

        [TextFilter("Clear", "Replaces the input text. (The same as ReplaceAll called with no arguments.)")]
        public static string Clear(string input, TextFilterCommand command)
        {
            return String.Empty;
        }

        [TextFilter("ReadFrom", "Sets the active text to the contents of a variable.")]
        [ArgumentMeta(1, "Variable Name", true, "The name of the variable to be retrieved.")]
        public static string ReadFrom(string input, TextFilterCommand command)
        {
            // This is a placeholder. No code will ever get here. See the "Execute" method of TextFilterPipeline.
            return String.Empty;
        }

        [TextFilter("WriteTo", "Writes the active text to the named variable.")]
        [ArgumentMeta(1, "Variable Name", true, "The name of the variable to which to write the input string.")]
        public static string WriteTo(string input, TextFilterCommand command)
        {
            // This is a placeholder. No code will ever get here. See the "Execute" method of TextFilterPipeline.
            return String.Empty;
        }
    }
}