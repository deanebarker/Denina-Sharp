using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DeninaSharp.Core
{
    public static class PipelineCommandParser
    {
        private const string COMMENT_PREFIX = "#";
        private const string PIPE_TOKEN = "=>";
        private const string APPEND_TOKEN = "==>";
        private const string VARIABLE_PREFIX = "$";
        private static readonly string COMMAND_DELIMITER = Environment.NewLine;

        public static IEnumerable<PipelineCommand> ParseCommandString(string commandString)
        {
            var commands = new List<PipelineCommand>();

            // If we have nothing, return an empty list of commands
            if (String.IsNullOrWhiteSpace(commandString))
            {
                return commands;
            }

            var lines = Regex.Split(commandString, @"\n(?!-| )")
                .Select(s => s.Replace(Environment.NewLine, " "))
                .Where(s => !String.IsNullOrWhiteSpace(s))
                .ToList();

            foreach (var line in lines)
            {
                // Is this a comment, or is it empty?
                if (String.IsNullOrWhiteSpace(line) || line.Trim().StartsWith(COMMENT_PREFIX))
                {
                    // Skip it...
                    continue;
                }

                var tokens = new TokenList(line);

                var command = new PipelineCommand()
                {
                    OriginalText = line
                };

                // $myVar =>
                if (tokens.Last == PIPE_TOKEN && tokens.Count == 2)
                {
                    command.FullyQualifiedCommandName = Pipeline.READ_FROM_VARIABLE_COMMAND;
                    command.OutputVariable = NormalizeVariableName(tokens.First);
                    commands.Add(command);
                    continue;
                }

                // => $myVar
                if (tokens.First == PIPE_TOKEN && tokens.Count == 2)
                {
                    command.FullyQualifiedCommandName = Pipeline.WRITE_TO_VARIABLE_COMMAND;
                    command.OutputVariable = NormalizeVariableName(tokens.Last);
                    commands.Add(command);
                    continue;
                }

                // WriteTo $myVar
                if (PipelineCommand.EnsureCategoryName(tokens.First.ToLower()) == Pipeline.WRITE_TO_VARIABLE_COMMAND)
                {
                    command.FullyQualifiedCommandName = Pipeline.WRITE_TO_VARIABLE_COMMAND;
                    command.OutputVariable = NormalizeVariableName(tokens.Second);
                    commands.Add(command);
                    continue;
                }

                // ReadFrom $myVar
                if (PipelineCommand.EnsureCategoryName(tokens.First.ToLower()) == Pipeline.READ_FROM_VARIABLE_COMMAND)
                {
                    command.FullyQualifiedCommandName = Pipeline.READ_FROM_VARIABLE_COMMAND;
                    command.InputVariable = NormalizeVariableName(tokens.Second);
                    commands.Add(command);
                    continue;
                }

                // DoSomething SomeArgument => $myVar
                if (tokens.Count > 2 && (tokens.SecondToLast == PIPE_TOKEN || tokens.SecondToLast == APPEND_TOKEN) && IsVariableName(tokens.Last))
                {
                    command.OutputVariable = NormalizeVariableName(tokens.Last());
                    command.AppendToOutput = tokens.SecondToLast == APPEND_TOKEN;

                    // Now, remove the last two items from the tokens and continue like normal...
                    tokens.RemoveFromEnd(2);
                }

                // $myVar => DoSomething
                if (tokens.Count > 2 && tokens.Second == PIPE_TOKEN && IsVariableName(tokens.First))
                {
                    command.InputVariable = NormalizeVariableName(tokens.First());

                    // Now, remove the last two items from the tokens and continue like normal...
                    tokens.RemoveFromStart(2);
                }

                // $myVar => DoSomething SomeArgument => $myVar
                // This situation will be handled by this point and the related tokens removed so that all that remains is (1) DoSomething, and (2) SomeArgument. From here, the command is parsed normally.

                // The first token is the command name...
                command.FullyQualifiedCommandName = tokens.First();

                //... the remaining tokens are the arguments.
                var counter = 0;
                foreach (var token in tokens.Skip(1))
                {
                    var value = token;
                    var key = counter.ToString();

                    if (Regex.IsMatch(token, "-[^:]+:.*"))
                    {
                        key = value.Substring(0, token.IndexOf(':')).Trim('-');
                        value = value.Substring(token.IndexOf(':') + 1);
                    }

                    if (command.CommandArgs.ContainsKey(key))
                    {
                        var multiVariableCounter = 1;
                        while (true)
                        {
                            var tempKey = String.Concat(key, ".", multiVariableCounter);
                            if (command.CommandArgs.ContainsKey(tempKey))
                            {
                                multiVariableCounter++;
                                continue;
                            }
                            key = tempKey;
                            break;
                        }
                    }

                    command.CommandArgs.Add(key, value.Trim('"'));
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
            var SPACE_PLACEHOLDER = "%%%%";

            var quoted = false;
            var modifiedText = new StringBuilder();
            foreach (var character in input)
            {
                // This this is a quote, toggle the quoted switch
                if (character == '"')
                {
                    quoted = !quoted;
                }

                // If we're inside quotes and this is a space, add the placeholder, otherwise add the character
                if (character == ' ' && quoted)
                {
                    modifiedText.Append(SPACE_PLACEHOLDER);
                }
                else
                {
                    modifiedText.Append(character);
                }
            }

            AddRange(
                Regex.Split(modifiedText.ToString(), @"\s+")
                    .Where(s => !String.IsNullOrWhiteSpace(s))
                    .Select(s => s.Replace(SPACE_PLACEHOLDER, " "))
                );
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