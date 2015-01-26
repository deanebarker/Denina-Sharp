using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BlendInteractive.TextFilterPipeline.Core
{
    public class TextFilterPipeline
    {
        private const string COMMENT_PREFIX = "#";
        private const string WRITE_TO_VARIABLE_COMMAND = "writeto";
        private const string READ_FROM_VARIABLE_COMMAND = "readfrom";
        private static readonly Dictionary<string, MethodInfo> commandMethods = new Dictionary<string, MethodInfo>();


        private readonly List<TextFilterCommand> pipeline = new List<TextFilterCommand>();

        private readonly Dictionary<string, object> variables = new Dictionary<string, object>();

        static TextFilterPipeline()
        {
            // Iterate all the classes in this assembly
            foreach (Type thisType in Assembly.GetExecutingAssembly().GetTypes())
            {
                // Does this assembly have the TextFilters attribute?
                if (thisType.GetCustomAttributes(typeof (TextFiltersAttribute), true).Any())
                {
                    // Process It
                    AddType(thisType);
                }
            }
        }

        public List<TextFilterCommand> Commands
        {
            get { return pipeline; }
        }

        public static Dictionary<string, MethodInfo> CommandMethods
        {
            get { return commandMethods; }
        }

        public Dictionary<string, object> Variables
        {
            get { return variables; }
        }

        public static void AddType(Type type)
        {
            string category = ((TextFiltersAttribute) type.GetCustomAttributes(typeof (TextFiltersAttribute), true).First()).Category.ToLower();

            foreach (MethodInfo method in type.GetMethods())
            {
                if (method.GetCustomAttributes(typeof (TextFilterAttribute), true).Any())
                {
                    string name = ((TextFilterAttribute) method.GetCustomAttributes(typeof (TextFilterAttribute), true).First()).Name;
                    commandMethods.Add(String.Concat(category, ".", name.ToLower()), method);
                }
            }
        }

        public string Execute(string input)
        {
            // If there's nothing to do, just send the same string back...
            if (!pipeline.Any())
            {
                return input;
            }

            foreach (var command in pipeline)
            {
                // Are we writing to a variable?
                if (command.CommandName == WRITE_TO_VARIABLE_COMMAND)
                {
                    var variableName = command.CommandArgs.First().Value;

                    if (variables.ContainsKey(variableName))
                    {
                        variables.Remove(variableName);
                    }

                    variables.Add(variableName, input);

                    continue;
                }

                // Are we reading from a variable?
                if (command.CommandName == READ_FROM_VARIABLE_COMMAND)
                {
                    var variableName = command.CommandArgs.First().Value;

                    if (variables.ContainsKey(variableName))
                    {
                        input = variables[variableName].ToString();
                    }
                    else
                    {
                        // Do we have a default argument as the second argument?
                        if (command.CommandArgs.Count > 1)
                        {
                            // Yes, use it
                            input = command.CommandArgs[1];
                        }
                        else
                        {
                            throw new Exception("Variable name \"" + variableName + "\" not found.");
                        }
                    }

                    continue;
                }


                // If it doesn't contain a dot, then put it in the "core" category
                if (!command.CommandName.Contains("."))
                {
                    command.CommandName = String.Concat("core.", command.CommandName);
                }

                // Do we have such a command?
                if (!CommandMethods.ContainsKey(command.CommandName))
                {
                    throw new Exception("No command method found for \"" + command.CommandName + "\"");
                }

                // Set a pipeline reference
                command.Pipeline = this;

                // Execute
                MethodInfo method = CommandMethods[command.CommandName];
                try
                {
                    input = (string) method.Invoke(null, new object[] {input, command});
                }
                catch (Exception e)
                {
                    throw new Exception(String.Concat(
                        method.Name,
                        ". ",
                        Environment.NewLine,
                        e.GetBaseException().Message,
                        Environment.NewLine,
                        e.GetBaseException().StackTrace));
                }
            }

            return input;
        }

        public object GetVariable(string key)
        {
            return variables[key];
        }

        public void AddCommand(string commandName, string defaultCommandArg, string variableName = null)
        {
            var command = new TextFilterCommand(commandName);
            command.CommandArgs = new Dictionary<object, string> {{"default", defaultCommandArg}};
            command.VariableName = variableName;
            pipeline.Add(command);
        }

        public void AddCommands(IEnumerable<TextFilterCommand> commands)
        {
            pipeline.AddRange(commands);
        }

        public void AddCommands(IEnumerable<string> commandLines)
        {
            foreach (string line in commandLines)
            {
                if (String.IsNullOrWhiteSpace(line) || line.Trim().StartsWith(COMMENT_PREFIX))
                {
                    continue;
                }

                pipeline.Add(new TextFilterCommand(line.Trim()));
            }
        }

        public void AddCommands(string commandString)
        {
            AddCommands(commandString.Trim().Split(Environment.NewLine.ToCharArray()));
        }

        public void AddCommand(TextFilterCommand command)
        {
            pipeline.Add(command);
        }

        public void AddCommand(string commandString)
        {
            AddCommand(new TextFilterCommand(commandString));
        }

        public void AddCommand(string commandName, Dictionary<object, string> commandArgs)
        {
            var command = new TextFilterCommand(commandName);
            command.CommandArgs = commandArgs;
            pipeline.Add(command);
        }
    }
}