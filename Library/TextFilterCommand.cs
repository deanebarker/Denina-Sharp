using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace BlendInteractive.TextFilterPipeline.Core
{
    public class TextFilterCommand
    {
        private string commandName;
        public string CommandName { get { return commandName; } set { commandName = value.ToLower().Trim(); } }
        public Dictionary<string, string> CommandArgs { get; set; }
        public string VariableName { get; set; }

        public TextFilterCommand()
        {
            CommandArgs = new Dictionary<string, string>();
        }

        public TextFilterCommand(string commandString)
        {
            CommandArgs = new Dictionary<string, string>();

            var tokens = GetTokens(commandString);

            CommandName = tokens.First().ToLower();
            var counter = 0;
            foreach (var command in tokens.Skip(1))
            {
                counter++;
                CommandArgs.Add(counter.ToString(), command);
            }
        }

        public string DefaultArgument
        {
            get { return CommandArgs.FirstOrDefault().Value; }
        }

        private List<string> GetTokens(string input)
        {
            var tokens = new List<string>();
            var rx = new Regex(@"(?<="")[^""]+(?="")|[^\s""]\S*");
            for (var match = rx.Match(input); match.Success; match = match.NextMatch())
            {
                tokens.Add(match.ToString());
            }

            return tokens;
        }


    }
}