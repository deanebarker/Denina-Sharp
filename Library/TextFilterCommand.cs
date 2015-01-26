using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BlendInteractive.TextFilterPipeline.Core
{
    public class TextFilterCommand
    {
        public TextFilterPipeline Pipeline;
        private string commandName;

        public TextFilterCommand()
        {
            CommandArgs = new Dictionary<object, string>();
        }

        public TextFilterCommand(string commandString)
        {
            CommandArgs = new Dictionary<object, string>();

            List<string> tokens = GetTokens(commandString);

            CommandName = tokens.First().ToLower();
            int counter = 0;
            foreach (string command in tokens.Skip(1))
            {
                CommandArgs.Add(counter, command);
                counter++;
            }
        }

        public string CommandName
        {
            get { return commandName; }
            set { commandName = value.ToLower().Trim(); }
        }

        public Dictionary<object, string> CommandArgs { get; set; }
        public string VariableName { get; set; }

        public string DefaultArgument
        {
            get { return CommandArgs.FirstOrDefault().Value; }
        }

        private List<string> GetTokens(string input)
        {
            var tokens = new List<string>();
            var rx = new Regex(@"(?<="")[^""]+(?="")|[^\s""]\S*");
            for (Match match = rx.Match(input); match.Success; match = match.NextMatch())
            {
                tokens.Add(match.ToString());
            }

            return tokens;
        }
    }
}