﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DeninaSharp.Core.Documentation;

namespace DeninaSharp.Core
{
    [DebuggerDisplay("{NormalizedCommandName}")]
    public class PipelineCommand
    {
        private const string DEFAULT_CATEGORY_NAME = "core";

        public Pipeline Pipeline;

        public PipelineCommand()
        {
            CommandArgs = new Dictionary<object, string>();
            InputVariable = Pipeline.GLOBAL_VARIABLE_NAME;
            OutputVariable = Pipeline.GLOBAL_VARIABLE_NAME;
            Label = Guid.NewGuid().ToString();  // This is a placeholder. In some instances, this will be reset.
        }

        public string FullyQualifiedCommandName { get; set; }
        public string CommandName => FullyQualifiedCommandName.Split(".".ToCharArray()).Last();
        public Dictionary<object, string> CommandArgs { get; set; }
        public string OutputVariable { get; set; }
        public string InputVariable { get; set; }
        public string OriginalText { get; set; }
        public string Label { get; set; } // For now, this just exists to identify the "end" command
        public string SendToLabel { get; set; }
        public bool AppendToOutput{ get; set; }

        public string CommandFactorySource { get; set; }

        public string DefaultArgument
        {
            get { return CommandArgs.FirstOrDefault().Value; }
        }

        public string NormalizedCommandName
        {
            get { return FullyQualifiedCommandName.Contains(".") ? FullyQualifiedCommandName.ToLower() : EnsureCategoryName(FullyQualifiedCommandName).ToLower(); }
        }

        public void ResolveArguments()
        {
            var method = Pipeline.CommandMethods[NormalizedCommandName];
            if (method.GetCustomAttributes(typeof (DoNotResolveVariablesAttribute), false).Any())
            {
                // We're not resolving attributes for this method. Bail out...
                return;
            }

            // This is a whole lot of crap to get around the problem of modifying a collection while it's being iterated...

            var variablesToResolve = new Dictionary<object, string>();
            foreach (var commandArg in CommandArgs)
            {
                if (PipelineCommandParser.IsVariableName(commandArg.Value))
                {
                    variablesToResolve.Add(commandArg.Key, commandArg.Value);
                }
            }

            foreach (var variable in variablesToResolve)
            {
                CommandArgs[variable.Key] = Pipeline.GetVariable(PipelineCommandParser.NormalizeVariableName(variable.Value)).ToString();
            }
        }

        public string GetArgument(int ordinal)
        {
            return CommandArgs[ordinal];
        }

        public string GetArgument(string key, string defaultValue = null)
        {
            foreach (var argName in key.Split(','))
            {
                if (CommandArgs.ContainsKey(argName))
                {
                    return CommandArgs[argName];
                }
            }

            if (defaultValue != null)
            {
                return defaultValue;
            }

            throw new DeninaException(String.Format("Attempt to access named argument \"{0}\" which was not provided.", key));
        }

        public List<string> GetMultiArgument(string key)
        {
            // Does the key exist at all?
            if (!CommandArgs.ContainsKey(key))
            {
                return new List<string>();
            }

            return CommandArgs.Where(a => a.Key.ToString() == key || a.Key.ToString().StartsWith(String.Concat(key, "."))).Select(a => a.Value).ToList();
        }

        public bool HasArgument(object key)
        {
            return CommandArgs.ContainsKey(key);
        }

        public static string EnsureCategoryName(string input)
        {
            return input.Contains(".") ? input : String.Concat(DEFAULT_CATEGORY_NAME, ".", input);
        }

        // The LogicHashCode is simply a hash of the command name and all the key/value combinations of the arguments in a standard order
        // This represents a unique value of the _logic_ of a command. The things hashed represent things that control the _logic_.
        // This can be used to avoid circular references when using Core.Include
        public int GetLogicHashCode()
        {
            var sb = new StringBuilder();
            sb.Append(NormalizedCommandName);
            sb.Append(string.Join(string.Empty, CommandArgs.OrderBy(a => a.ToString()).Select(a => string.Concat(a.Key.ToString().ToLower(), a.Value))));
            return sb.ToString().GetHashCode();
        }


    }
}