using System;
using System.Linq;
using System.Web.UI;
using DeninaSharp.Core.Documentation;
using System.IO;
using System.Xml;
using System.Text;

namespace DeninaSharp.Core.Filters
{
    [Filters("Core", "For working with the pipeline and its variables.")]
    public static class Core
    {
        [Filter("Clear", "Replaces the input text. (The same as ReplaceAll called with no arguments.)")]
        [CodeSample("(Anything)", "Core.Clear", "(Nothing)")]
        public static string Clear(string input, PipelineCommand command)
        {
            return String.Empty;
        }

        [Filter("ReadFrom", "Sets the active text to the contents of a variable. Used to retrieve variable values from storage.")]
        [ArgumentMeta("var", true, "The name of the variable to be retrieved.")]
        [CodeSample("(Anything, with the \"Name\" variable holding the value \"James Bond\")", "Core.ReadFrom Name", "James Bond")]
        [DoNotResolveVariables]
        public static string ReadFrom(string input, PipelineCommand command)
        {
            // This is a placeholder. No code will ever get here. See the "Execute" method of TextFilterPipeline.
            return string.Empty;
        }

        [Filter("WriteTo", "Writes the active text to the named variable. Used to push the active text into variable storage.")]
        [ArgumentMeta("var", true, "The name of the variable to which to write the input string.")]
        [CodeSample("James Bond", "Core.WriteTo Name", "(The variable \"Name\" now contains \"James Bond\".)")]
        [DoNotResolveVariables]
        public static string WriteTo(string input, PipelineCommand command)
        {
            // This is a placeholder. No code will ever get here. See the "Execute" method of TextFilterPipeline.
            return String.Empty;
        }

        [Filter("SetVar", "Sets the value of a variable to the value provided. Does not change the input string.")]
        [ArgumentMeta("var", true, "The name of the variable to set.")]
        [ArgumentMeta("value", false, "The desired value. If not provided, the variable is set to an empty string (same as InitVar).")]
        [CodeSample("(Anything)", "Core.SetVar -var:Name -value:\"James Bond\"\nReadFrom Name", "James Bond")]
        [DoNotResolveVariables]
        public static string SetVar(string input, PipelineCommand command)
        {
            var variableName = command.GetArgument("var");
            var value = command.GetArgument("value", String.Empty);
            
            command.Pipeline.SafeSetVariable(variableName, value);

            return input;
        }

        [Filter("InitVar", "Sets the value of a variable to an empty string. The variable can now be referenced without error.")]
        [ArgumentMeta("var", true, "The name of the variable to set. Multiple variables can be specified. All will be initialized.")]
        [DoNotResolveVariables]
        [CodeSample("(Anything)", "Core.InitVar -var:Name -var:Address -var:City -var:State -var:Zip", "(Unchanged; no output. However, the named variables are all initialized to empty strings.)")]
        public static string InitVar(string input, PipelineCommand command)
        {
            foreach (var variableName in command.GetMultiArgument("var"))
            {
                if (!command.Pipeline.Variables.ContainsKey(variableName))
                {
                    command.Pipeline.SafeSetVariable(variableName, String.Empty);
                }
            }
            return input;
        }

        [Filter("Now", "Returns the current date and time, formatted by an optional format string.")]
        [ArgumentMeta("format", false, "The C# time format string with which to format the results.")]
        [CodeSample("(Anything)", "Core.Now -format:\"ddd d MMM\"", "Wed 25 Feb")]
        public static string Now(string input, PipelineCommand command)
        {
            var formatString = "f";
            if (command.CommandArgs.Count == 1)
            {
                formatString = command.GetArgument("format");
            }
            return DateTime.Now.ToString(formatString);
        }

        [Filter("AppendVar", "Appends to the value of a variable in storage.")]
        [ArgumentMeta("var", true, "The name of the variable to which to append data.")]
        [ArgumentMeta("value", false, "The value to append. If omitted, the input string will be appended.")]
        [CodeSample("(Anything)", "Core.SetVar -var:Name -value:James\nCore.AppendVar -var:Name -value:\" Bond\"\nCore.ReadFrom Name", "James Bond")]
        public static string AppendVar(string input, PipelineCommand command)
        {
            // If they didn't provide a value, then use the input
            var valueToAppend = command.GetArgument("value", input);
            var variableName = command.GetArgument("var");

            var oldValue = command.Pipeline.GetVariable(variableName);
            var newValue = String.Concat(oldValue, valueToAppend);

            command.Pipeline.SafeSetVariable(variableName, newValue);

            return input;
        }

        [Filter("Label", "A placeholder filter for a label. Simply passes text through.")]
        public static string Pass(string input, PipelineCommand command)
        {
            return input;
        }

        [Filter("End", "Ends execution.")]
        [CodeSample("(Anything)", "Core.End", "(Execution ends, and the active text is passed out of the pipeline as if this was the last command.)")]
        public static string End(string input, PipelineCommand command)
        {
            command.SendToLabel = "end";
            return input;
        }

        [Filter("Dump", "Dumps debug information to output.")]
        [ArgumentMeta("io", false, "Include input and output data (this can get long). Defaults to \"true\".)")]
        [ArgumentMeta("variables", false, "Include variables (this can get long). Defaults to \"true\".)")]
        public static string Dump(string input, PipelineCommand command)
        {

            Boolean.TryParse(command.GetArgument("io", "true"), out bool includeIo);
            Boolean.TryParse(command.GetArgument("variables", "true"), out bool includeVariables);

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var writer = new XmlTextWriter(sw);

            writer.Formatting = Formatting.Indented;
            writer.Indentation = 4;

            writer.WriteStartDocument();
            writer.WriteStartElement("pipelineDebugData");

            foreach (var debugEntry in command.Pipeline.DebugData)
            {
                writer.WriteStartElement("commandDebugData");

                writer.WriteElementString("commandName", debugEntry.CommandName);
                writer.WriteElementString("commandText", debugEntry.CommandText);

                writer.WriteStartElement("arguments");
                foreach(var argument in debugEntry.Arguments)
                {
                    writer.WriteStartElement("argument");
                    writer.WriteAttributeString("key", argument.Key.ToString());
                    writer.WriteCData(argument.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                if (includeIo)
                {
                    writer.WriteStartElement("inputValue");
                    writer.WriteCData(debugEntry.InputValue);
                    writer.WriteEndElement();

                    writer.WriteStartElement("outputValue");
                    writer.WriteCData(debugEntry.OutputValue);
                    writer.WriteEndElement();
                }

                if (includeVariables)
                {
                    writer.WriteStartElement("variables");
                    foreach (var variable in debugEntry.Variables)
                    {
                        writer.WriteStartElement("variable");
                        writer.WriteAttributeString("key", variable.Key);
                        writer.WriteCData(variable.Value);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }

                writer.WriteElementString("successfullyExecuted", debugEntry.SuccessfullyExecuted.ToString());
                writer.WriteElementString("elapsedTime", debugEntry.ElapsedTime.ToString());

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();

            return sb.ToString();
        }
        
    }
}