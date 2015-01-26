using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;

namespace BlendInteractive.TextFilterPipeline.Core
{
    public static class CommandParser
    {
        private const string COMMENT_PREFIX = "#";
        private const string VARIABLE_DELIMITER = "=>";
        
        public static IEnumerable<TextFilterCommand> ParseCommandString(string commandString)
        {
            var commands = new List<TextFilterCommand>();
            foreach (var line in commandString.Trim().Split(Environment.NewLine.ToCharArray()).Select(s => s.Trim()))
            {
                if (String.IsNullOrWhiteSpace(line) || line.Trim().StartsWith(COMMENT_PREFIX))
                {
                    continue;
                }

                // Do we have a variable output?
                String variableName = null;
                List<string> tokens;
                if (line.Contains(VARIABLE_DELIMITER))
                {
                    var parts = Regex.Split(line, VARIABLE_DELIMITER);
                    variableName = parts.Last().Trim();
                    tokens = GetTokens(parts.First().Trim());
                }
                else
                {
                    tokens = GetTokens(line);   
                }

                var command = new TextFilterCommand()
                {
                    // The first token is the command name (every line that gets here should have at least one token)...
                    CommandName = tokens.First(),
                    VariableName = variableName
                };

                //... the remaining tokens are the arguments.
                var counter = 0;
                foreach (var token in tokens.Skip(1))
                {
                    command.CommandArgs.Add(counter, token);
                    counter++;
                }

                commands.Add(command);
            }

            return commands;
        }

        private static List<string> GetTokens(string input)
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
