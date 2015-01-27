using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BlendInteractive.TextFilterPipeline.Core
{
    public class TextFilterPipeline
    {
        
        private const string WRITE_TO_VARIABLE_COMMAND = "writeto";
        private const string READ_FROM_VARIABLE_COMMAND = "readfrom";
        private static readonly Dictionary<string, MethodInfo> commandMethods = new Dictionary<string, MethodInfo>();
        
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

        public TextFilterPipeline(string commandString = null)
        {
            if (!String.IsNullOrWhiteSpace(commandString))
            {
                AddCommand(commandString);
            }
        }

        public static Dictionary<string, MethodInfo> CommandMethods
        {
            get { return commandMethods; }
        }
        
        private readonly List<TextFilterCommand> commands = new List<TextFilterCommand>();
        public List<TextFilterCommand> Commands
        {
            get { return commands; }
        }

        private readonly Dictionary<string, object> variables = new Dictionary<string, object>();
        public Dictionary<string, object> Variables
        {
            get { return variables; }
        }

        public static void AddType(Type type)
        {
            if (!type.GetCustomAttributes(typeof (TextFiltersAttribute), true).Any())
            {
                throw new Exception("Type does not have a TextFilters attribute. In this case, you mist pass a category name into AddType.");
            }

            var category = ((TextFiltersAttribute) type.GetCustomAttributes(typeof (TextFiltersAttribute), true).First()).Category;
            AddType(category, type);
        }

        public static void AddType(string category, Type type)
        {
            foreach (MethodInfo method in type.GetMethods())
            {
                if (method.GetCustomAttributes(typeof(TextFilterAttribute), true).Any())
                {
                    string name = ((TextFilterAttribute)method.GetCustomAttributes(typeof(TextFilterAttribute), true).First()).Name;

                    var fullyQualifiedFilterName = String.Concat(category.ToLower(), ".", name.ToLower());

                    commandMethods.Remove(fullyQualifiedFilterName);    // Remove it if it exists already                  
                    commandMethods.Add(fullyQualifiedFilterName, method);
                }
            }
        }

        public string Execute(string input = null)
        {
            // It's perfectly valid to call it without passing anything in. This assumes that a command will acquire text somehow.
            if (input == null)
            {
                input = String.Empty;
            }

            // If there's nothing to do, just send the same string back...
            if (!commands.Any())
            {
                return input;
            }

            foreach (TextFilterCommand command in commands)
            {
                // Are we writing to a variable?
                if (command.NormalizedCommandName == WRITE_TO_VARIABLE_COMMAND)
                {
                    string variableName = command.CommandArgs.First().Value;

                    if (variables.ContainsKey(variableName))
                    {
                        variables.Remove(variableName);
                    }

                    variables.Add(variableName, input);

                    continue;
                }

                // Are we reading from a variable?
                if (command.NormalizedCommandName == READ_FROM_VARIABLE_COMMAND)
                {
                    string variableName = command.CommandArgs.First().Value;

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
                if (!command.NormalizedCommandName.Contains("."))
                {
                    command.CommandName = String.Concat("core.", command.CommandName);
                }

                // Do we have such a command?
                if (!CommandMethods.ContainsKey(command.NormalizedCommandName))
                {
                    throw new Exception("No command method found for \"" + command.CommandName + "\"");
                }

                // Set a pipeline reference
                command.Pipeline = this;

                // Execute
                MethodInfo method = CommandMethods[command.NormalizedCommandName];
                try
                {
                    // This is where we make the method call
                    var result = (string) method.Invoke(null, new object[] {input, command});

                    if (!String.IsNullOrWhiteSpace(command.VariableName))
                    {
                        WriteToVariable(command.VariableName, result);
                    }
                    else
                    {
                        // If we're not writing to a variable, then we're changing the active text
                        input = result;
                    }
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

        public void WriteToVariable(string key, string value)
        {
            variables.Remove(key);
            variables.Add(key, value);
        }

        public object GetVariable(string key)
        {
            return variables[key];
        }

        public void AddCommand(TextFilterCommand command)
        {
            commands.Add(command);
        }

        public void AddCommand(string commandString)
        {
            commands.AddRange(CommandParser.ParseCommandString(commandString));
        }

        public void AddCommand(string commandName, Dictionary<object, string> commandArgs)
        {
            var command = new TextFilterCommand()
            {
                CommandName = commandName,
                CommandArgs = commandArgs
            };
            commands.Add(command);
        }

    }
}