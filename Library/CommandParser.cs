using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;

namespace BlendInteractive.TextFilterPipeline.Core
{
    public static class CommandParser
    {
        private const string COMMENT_PREFIX = "#";
        private const string PIPE_TOKEN = "=>";
        private const string VARIABLE_PREFIX = "$";
        private static readonly string COMMAND_DELIMITER = Environment.NewLine;
        
        public static IEnumerable<TextFilterCommand> ParseCommandString(string commandString)
        {
            var commands = new List<TextFilterCommand>();
            foreach (var line in commandString.Trim().Split(COMMAND_DELIMITER.ToCharArray()).Select(s => s.Trim()))
            {
                // Is this a comment, or is it empty?
                if (String.IsNullOrWhiteSpace(line) || line.Trim().StartsWith(COMMENT_PREFIX))
                {
                    // Skip it...
                    continue;
                }

                var tokens = new TokenList(line);

                var command = new TextFilterCommand();

                // $myVar =>
                if (tokens.Last == PIPE_TOKEN && IsVariableName(tokens.First) && tokens.Count == 2)
                {
                    command.CommandName = TextFilterPipeline.READ_FROM_VARIABLE_COMMAND;
                    command.OutputVariable = tokens.First;
                    commands.Add(command);
                    continue;
                }

                // => $myVar
                if (tokens.First == PIPE_TOKEN && IsVariableName(tokens.Last) && tokens.Count == 2)
                {
                    command.CommandName = TextFilterPipeline.WRITE_TO_VARIABLE_COMMAND;
                    command.OutputVariable = tokens.Last;
                    commands.Add(command);
                    continue;
                }

                // WriteTo $myVar
                if (TextFilterCommand.EnsureCategoryName(tokens.First.ToLower()) == TextFilterPipeline.WRITE_TO_VARIABLE_COMMAND && IsVariableName(tokens.Second))
                {
                    command.CommandName = TextFilterPipeline.WRITE_TO_VARIABLE_COMMAND;
                    command.OutputVariable = tokens.Second;
                    commands.Add(command);
                    continue;
                }

                // ReadFrom $myVar
                if (TextFilterCommand.EnsureCategoryName(tokens.First.ToLower()) == TextFilterPipeline.READ_FROM_VARIABLE_COMMAND && IsVariableName(tokens.Second))
                {
                    command.CommandName = TextFilterPipeline.READ_FROM_VARIABLE_COMMAND;
                    command.OutputVariable = tokens.Second;
                    commands.Add(command);
                    continue;
                }

                // DoSomething SomeArgument => $myVar
                if (tokens.Count > 2 && tokens.SecondToLast == PIPE_TOKEN && IsVariableName(tokens.Last))
                {
                    command.OutputVariable = tokens.Last();
                    
                    // Now, remove the last two items from the tokens and continue like normal...
                    tokens.RemoveFromEnd(2);
                }

               // $myVar => DoSomething
                if (tokens.Count > 2 && tokens.Second == PIPE_TOKEN && IsVariableName(tokens.First))
                {
                    command.InputVariable = tokens.First();

                    // Now, remove the last two items from the tokens and continue like normal...
                    tokens.RemoveFromStart(2);
                }

				// $myVar => DoSomething SomeArgument => $myVar
				// This situation will be handled by this point and the related tokens removed so that all that remains is (1) DoSomething, and (2) SomeArgument. From here, the command is parsed normally.

                // The first token is the command name...
                command.CommandName = tokens.First();

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

        internal static bool IsVariableName(string p)
        {
            return p.StartsWith(VARIABLE_PREFIX);
        }

        internal static string NormalizeVariableName(string p)
        {
            return p.TrimStart(VARIABLE_PREFIX.ToCharArray());
        }
    }

    internal class TokenList : List<string>
    {
        public TokenList(string input)
        {
           var rx = new Regex(@"(?<="")[^""]+(?="")|[^\s""]\S*");
            for (Match match = rx.Match(input); match.Success; match = match.NextMatch())
            {
                Add(match.ToString());
            }           
        }

        public string First
        {
            get { return this.First(); }
        }

        public string Second
        {
            get { return GetValue(1); }
        }

        public string Third
        {
            get { return GetValue(2); }
        }

        public string Fourth
        {
            get { return GetValue(3); }
        }

        public string Fifth
        {
            get { return GetValue(4); }
        }

        public string Last
        {
            get { return this.Last(); }
        }

        public string SecondToLast
        {
            get
            {
                if (this.Count == 1)
                {
                    return null;
                }

                return this.Skip(this.Count - 2).First();
            }
        }

        public void RemoveFromEnd(int count)
        {
            if (count >= this.Count)
            {
                this.Clear();
            }


            for(var index = 0; index < count; index++)
            {
                this.RemoveAt(this.Count - 1);
            }
        }


        public void RemoveFromStart(int count)
        {
            if (count >= this.Count)
            {
                this.Clear();
            }


            for (var index = 0; index < count; index++)
            {
                this.RemoveAt(0);
            }
        }

        private string GetValue(int index)
        {
            return this.Count > index ? this[index] : null;
        }
    }
}
