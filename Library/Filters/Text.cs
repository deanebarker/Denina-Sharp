using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using BlendInteractive.Denina.Core.Documentation;
using DeninaSharp.Core.Documentation;

namespace DeninaSharp.Core.Filters
{
    [Filters("Text", "For working with text.")]
    public static class Text
    {
        [Filter("Append", "Appends a string to the input string.")]
        [ArgumentMeta("suffix", true, "The string to append to the input string.")]
        [CodeSample("James", "Text.Append \" Bond\"", "James Bond")]
        public static string Append(string input, PipelineCommand command)
        {
            return String.Concat(input, command.GetArgument("suffix"));
        }

        [Filter("Prepend", "Prepends a string to the input string.")]
        [ArgumentMeta("prefix", true, "The string to prepend to the input string.")]
        [CodeSample("Bond", "Text.Append \"James \"", "James Bond")]
        public static string Prepend(string input, PipelineCommand command)
        {
            return String.Concat(command.GetArgument("prefix"), input);
        }

        [Filter("Concat", "Concatenates a series of arguments together.")]
        [ArgumentMeta("value", true, "Any value (variable or literal). This argument may be repeated multiple times. All passed-in arguments will be concatenated.")]
        [CodeSample("", "Text.Append James Bond was here", "JamesBondwashere")]
        public static string Concat(string input, PipelineCommand command)
        {
            var arguments = command.GetMultiArgument("value");
            return String.Join(String.Empty, arguments);
        }


        [Filter("Replace", "Finds and replaces one string with another.")]
        [ArgumentMeta("old", true, "The string to be replaced.")]
        [ArgumentMeta("new", false, "The new string. If this argument is not provided, String.Empty will be used (so, the Old String will simply be removed.)")]
        [CodeSample("James Bond", "Text.Replace James \"Bond. James\"", "Bond. James Bond")]
        [CodeSample("James Bond", "Text.Replace \"James \"", "Bond")]
        public static string Replace(string input, PipelineCommand command)
        {
            var oldValue = command.GetArgument("old");
            var newValue = command.GetArgument("new", String.Empty);

            var respectCase = false;
            Boolean.TryParse(command.GetArgument("case", "false"), out respectCase);

            return respectCase ? input.Replace(oldValue, newValue) :  Regex.Replace(input, oldValue, newValue, RegexOptions.IgnoreCase);
        }

        [Filter("ReplaceAll", "Completely replaces the input string.")]
        [ArgumentMeta("text", false, "The new string. After this filter executes, the active text will be set to this value. If no argument is provided, String.Empty is used, effectively clearing the active text.")]
        [CodeSample("(Any)", "ReplaceAll \"James Bond\"", "James Bond")]
        [CodeSample("(Any)", "ReplaceAll", "(None)")]
        public static string ReplaceAll(string input, PipelineCommand command)
        {
            // ReplaceAll with no arguments is essentially the same as Clear
            if (!command.CommandArgs.Any())
            {
                return String.Empty;
            }
            return command.GetArgument("text");
        }

        [Filter("Format", "Performs simple token replacement of variables and the active text.")]
        [ArgumentMeta("template", true, "A format string suitable for usage in String.Format. The input string will replace the {0} token. Variable values will replace {variableName} tokens.")]
        [CodeSample("James Bond", "Text.Format \"My name is {0}.\"", "My name is James Bond.")]
        [CodeSample(
            "",
            @"SetVar Name ""James Bond""
            Text.Format ""My name is {Name}""",
            "My name is James Bond.")]
        public static string Format(string input, PipelineCommand command)
        {
            var template = command.GetArgument("template");

            foreach (var variable in command.Pipeline.Variables)
            {
                template = template.Replace(String.Concat("{", variable.Key, "}"), Convert.ToString(variable.Value));
            }

            // Escape any remaining {string} patterns, because these will break String.Format...
            template = Regex.Replace(template, "{([^0-9]+)}", "{{$1}}");

            return String.Format(template, input);
        }

        [Filter("Trim", "Trims whitespace from the ends of the input string.")]
        public static string Trim(string input, PipelineCommand command)
        {
            return input.Trim();
        }

        [Filter("FormatLines", "Performs a template formatting operation on every line in a string and concatenates the result.")]
        [ArgumentMeta("template", true, "A format string suitable for usage in String.Format. Each line will replace the {0} token.")]
        [CodeSample("James Bond\nErnst Blofeld", "Text.FormatLines &lt;li&gt;{0}&lt;/li&gt;", "&lt;li&gt;James Bond&lt;/li&gt;&lt;li&gt;Ernst Blofeld&lt;/li&gt;")]
        public static string FormatLines(string input, PipelineCommand command)
        {
            var returnString = new StringBuilder();
            foreach (var line in input.Split(new [] { Environment.NewLine }, StringSplitOptions.None))
            {
                returnString.AppendFormat(command.GetArgument("template"), line);
            }
            return returnString.ToString();
        }

        [Filter("ExtractRegex", "Extracts a substring based on a supplied regex.")]
        [ArgumentMeta("pattern", true, "A regex string pattern. It will require an extraction expression.")]
        [CodeSample("James Bond", "Text.ExtractRegex a(..)s", "me")]
        public static string ExtractRegex(string input, PipelineCommand command)
        {
            var result = Regex.Match(input, command.GetArgument("pattern"));

            if (result.Groups.Count == 0)
            {
                return String.Empty;
            }

            return result.Groups[1].Value;
        }
    }
}