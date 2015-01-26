using System;

namespace BlendInteractive.TextFilterPipeline.Core.Filters
{
    [TextFilters("Core")]
    public static class Core
    {
        [TextFilter("Append")]
        public static string Append(string input, TextFilterCommand command)
        {
            return String.Concat(input, command.DefaultArgument);
        }

        [TextFilter("Prepend")]
        public static string Prepend(string input, TextFilterCommand command)
        {
            return String.Concat(command.DefaultArgument, input);
        }


        [TextFilter("Replace")]
        public static string Replace(string input, TextFilterCommand command)
        {
            // If they didn't pass in a second argument, then we're using String.Empty (so, less "replace" and more "remove"
            var replaceWith = command.CommandArgs.Count > 1 ? command.CommandArgs[1] : String.Empty;

            return input.Replace(command.CommandArgs[0], replaceWith);
        }

        [TextFilter("ReplaceAll")]
        public static string ReplaceAll(string input, TextFilterCommand command)
        {
            return command.DefaultArgument;
        }

        [TextFilter("Format")]
        public static string Format(string input, TextFilterCommand command)
        {
            string template = command.CommandArgs.ContainsKey(0) ? command.CommandArgs[0] : String.Empty;
            template = command.CommandArgs.ContainsKey("template") && !String.IsNullOrWhiteSpace(command.CommandArgs["template"]) ? command.CommandArgs["template"] : template;

            foreach (var variable in command.Pipeline.Variables)
            {
                template = template.Replace(String.Concat("{", variable.Key, "}"), variable.Value.ToString());
            }

            return String.Format(template, input);
        }

        [TextFilter("Trim")]
        public static string Trim(string input, TextFilterCommand command)
        {
            return input.Trim();
        }

        [TextFilter("Clear")]
        public static string Clear(string input, TextFilterCommand command)
        {
            return String.Empty;
        }
    }
}