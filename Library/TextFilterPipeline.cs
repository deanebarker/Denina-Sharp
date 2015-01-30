using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using Newtonsoft.Json.Converters;

namespace BlendInteractive.TextFilterPipeline.Core
{
    public class TextFilterPipeline
    {
        public const string GLOBAL_VARIABLE_NAME = "__global";
        public const string WRITE_TO_VARIABLE_COMMAND = "core.writeto";
        public const string READ_FROM_VARIABLE_COMMAND = "core.readfrom";
        
        public static readonly Dictionary<string, Type> Types = new Dictionary<string, Type>();    // This is just to keep them handy for the documentor

        public TextFilterPipeline(string commandString = null)
        {
            // Add this assembly to initialze the filters
            AddAssembly(Assembly.GetExecutingAssembly());

            if (!String.IsNullOrWhiteSpace(commandString))
            {
                AddCommand(commandString);
            }
        }

        private static readonly Dictionary<string, MethodInfo> commandMethods = new Dictionary<string, MethodInfo>();
        public static ReadOnlyDictionary<string, MethodInfo> CommandMethods
        {
            get { return new ReadOnlyDictionary<string, MethodInfo>(commandMethods); }
        }
        
        private readonly List<TextFilterCommand> commands = new List<TextFilterCommand>();
        public ReadOnlyCollection<TextFilterCommand> Commands
        {
            get { return commands.AsReadOnly(); }
        }

        private readonly Dictionary<string, object> variables = new Dictionary<string, object>();
        public ReadOnlyDictionary<string, object> Variables
        {
            get { return new ReadOnlyDictionary<string, object>(variables); }
        }

        public static void AddAssembly(Assembly assembly)
        {
            // Iterate all the classes in this assembly
            foreach (var thisType in assembly.GetTypes())
            {
                // Does this assembly have the TextFilters attribute?
                if (thisType.GetCustomAttributes(typeof(TextFiltersAttribute), true).Any())
                {
                    // Process It
                    AddType(thisType);
                }
            }
        }

        public static void AddType(Type type, string category = null)
        {
            if (category == null)
            {
                if (!type.GetCustomAttributes(typeof (TextFiltersAttribute), true).Any())
                {
                    throw new Exception("Type does not have a TextFilters attribute. In this case, you must pass a category name into AddType.");
                }
                category = ((TextFiltersAttribute) type.GetCustomAttributes(typeof (TextFiltersAttribute), true).First()).Category;

            }

            foreach (var method in type.GetMethods().Where(m => m.GetCustomAttributes(typeof (TextFilterAttribute), true).Any()))
            {
                var name = ((TextFilterAttribute)method.GetCustomAttributes(typeof(TextFilterAttribute), true).First()).Name;
                AddMethod(method, category, name);
            }
        }

        public static void AddMethod(MethodInfo method, string category, string name = null)
        {
            if (name == null)
            {
                name = ((TextFilterAttribute)method.GetCustomAttributes(typeof(TextFilterAttribute), true).First()).Name;
            }
            
            var fullyQualifiedFilterName = String.Concat(category.ToLower(), ".", name.ToLower());

            commandMethods.Remove(fullyQualifiedFilterName);    // Remove it if it exists already                  
            commandMethods.Add(fullyQualifiedFilterName, method);            
        }

        public string Execute(string input = null)
        {
            // We set the global variable to the incoming string. It will be modified and eventually returned from this variable slot.
            SetVariable(GLOBAL_VARIABLE_NAME, input);

            foreach (var command in commands)
            {
                // Are we writing to a variable?
                if (command.NormalizedCommandName == WRITE_TO_VARIABLE_COMMAND)
                {
                    SetVariable(command.OutputVariable, input);
                    continue;
                }

                // Are we reading from a variable?
                if (command.NormalizedCommandName == READ_FROM_VARIABLE_COMMAND)
                {
                    SetVariable(GLOBAL_VARIABLE_NAME, GetVariable(command.OutputVariable));
                    continue;
                }

                // Note that the WRITE_TO_VARIABLE_COMMAND and READ_FROM_VARIABLE_COMMAND commands will never actually be executed. This is why their methods are just empty shells...

                // Do we have such a command?
                if (!CommandMethods.ContainsKey(command.NormalizedCommandName))
                {
                    throw new TfpException("No command method found for \"" + command.CommandName + "\"");
                }

                // Set a pipeline reference which can be accessed inside the filter method
                command.Pipeline = this;

                // Resolve any arguments that are actually variable names
                command.ResolveArguments();

                // Execute
                var method = CommandMethods[command.NormalizedCommandName];
                try
                {
                    // This is where we make the actual method call. We get the text out of the InputVariable slot, and we put it back into the OutputVariable slot. (These are usually the same slot...)
                    SetVariable(
                        command.OutputVariable,
                        method.Invoke( null, new [] { GetVariable(command.InputVariable), command } )
                    );
                }
                catch (Exception e)
                {
                    throw new TfpException(String.Concat(
                        "Error in filter: \"",
                        command.NormalizedCommandName,
                        "\". ",
                        Environment.NewLine,
                        e.GetBaseException().Message,
                        Environment.NewLine,
                        e.GetBaseException().StackTrace));
                }

                // Move to the next command...
            }

            // Return what's in the global variable            
            return GetVariable(GLOBAL_VARIABLE_NAME).ToString();
        }

        public object GetVariable(string key)
        {
            key = CommandParser.NormalizeVariableName(key);

            if (!variables.ContainsKey(key))
            {
                throw new TfpException(String.Format("Attempt to access non-existent variable: \"{0}\"", key));
            }

            return variables[CommandParser.NormalizeVariableName(key)];
        }

        public void SetVariable(string key, object value)
        {
            key = CommandParser.NormalizeVariableName(key);
            variables.Remove(key);
            variables.Add(key, value);
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