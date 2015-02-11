using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BlendInteractive.Denina.Core
{
    public static class PipelineCommandParser
    {
        private const string COMMENT_PREFIX = "#";
        private const string PIPE_TOKEN = "=>";
        private const string VARIABLE_PREFIX = "$";
        private static readonly string COMMAND_DELIMITER = Environment.NewLine;

        public static IEnumerable<PipelineCommand> ParseCommandString(string commandString)
        {
            var commands = new List<PipelineCommand>();
            foreach (var line in commandString.Trim().Split(COMMAND_DELIMITER.ToCharArray()).Select(s => s.Trim()))
            {
                // Is this a comment, or is it empty?
                if (String.IsNullOrWhiteSpace(line) || line.Trim().StartsWith(COMMENT_PREFIX))
                {
                    // Skip it...
                    continue;
                }

                var tokens = new TokenList(line);

                var command = new PipelineCommand();
                command.OriginalText = line;

                // $myVar =>
                if (tokens.Last == PIPE_TOKEN && IsVariableName(tokens.First) && tokens.Count == 2)
                {
                    command.CommandName = Pipeline.READ_FROM_VARIABLE_COMMAND;
                    command.OutputVariable = tokens.First;
                    commands.Add(command);
                    continue;
                }

                // => $myVar
                if (tokens.First == PIPE_TOKEN && IsVariableName(tokens.Last) && tokens.Count == 2)
                {
                    command.CommandName = Pipeline.WRITE_TO_VARIABLE_COMMAND;
                    command.OutputVariable = tokens.Last;
                    commands.Add(command);
                    continue;
                }

                // WriteTo $myVar
                if (PipelineCommand.EnsureCategoryName(tokens.First.ToLower()) == Pipeline.WRITE_TO_VARIABLE_COMMAND && IsVariableName(tokens.Second))
                {
                    command.CommandName = Pipeline.WRITE_TO_VARIABLE_COMMAND;
                    command.OutputVariable = tokens.Second;
                    commands.Add(command);
                    continue;
                }

                // ReadFrom $myVar
                if (PipelineCommand.EnsureCategoryName(tokens.First.ToLower()) == Pipeline.READ_FROM_VARIABLE_COMMAND && IsVariableName(tokens.Second))
                {
                    command.CommandName = Pipeline.READ_FROM_VARIABLE_COMMAND;
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
            for (var match = rx.Match(input); match.Success; match = match.NextMatch())
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
            get { return Count == 1 ? null : this.Skip(Count - 2).First(); } 
        }

        public void RemoveFromEnd(int count)
        {
            if (count >= Count)
            {
                Clear();
            }

            for (var index = 0; index < count; index++)
            {
                RemoveAt(Count - 1);
            }
        }


        public void RemoveFromStart(int count)
        {
            if (count >= Count)
            {
                Clear();
            }


            for (var index = 0; index < count; index++)
            {
                RemoveAt(0);
            }
        }

        private string GetValue(int index)
        {
            return Count > index ? this[index] : null;
        }
    }
}