using System;

namespace BlendInteractive.TextFilterPipeline.Core.Filters
{
    [TextFilters("Core")]
    public static class Core
    {
        [TextFilter("append")]
        public static string Append(string input, TextFilterCommand command)
        {
            return String.Concat(input, command.DefaultArgument);
        }

        [TextFilter("prepend")]
        public static string Prepend(string input, TextFilterCommand command)
        {
            return String.Concat(command.DefaultArgument, input);
        }


        [TextFilter("replace")]
        public static string Replace(string input, TextFilterCommand command)
        {
            return input.Replace(command.CommandArgs[0], command.CommandArgs[1]);
        }

        [TextFilter("replaceall")]
        public static string ReplaceAll(string input, TextFilterCommand command)
        {
            return command.DefaultArgument;
        }

        [TextFilter("format")]
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

        [TextFilter("trim")]
        public static string Trim(string input, TextFilterCommand command)
        {
            return input.Trim();
        }
    }
}