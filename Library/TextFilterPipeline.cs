using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;
using System.Xml.Xsl;

namespace BlendInteractive.TextFilterPipeline.Core
{

    public class TextFilterPipeline
    {
        private const string COMMENT_PREFIX = "#";

        private List<TextFilterCommand> pipeline = new List<TextFilterCommand>();

        public List<TextFilterCommand> Commands
        {
            get { return pipeline; }
        }

        private Dictionary<string, MethodInfo> CommandMethods;

        public TextFilterPipeline()
        {
            CommandMethods = new Dictionary<string, MethodInfo>();
            LoadMethodTable();
        }

        public void LoadMethodTable()
        {
            foreach (var method in GetType().GetMethods())
            {
                foreach (var attribute in method.GetCustomAttributes())
                {
                    if (attribute is TextFilterAttribute)
                    {
                        var commandAttribute = (TextFilterAttribute)attribute;
                        CommandMethods.Add(commandAttribute.Name.ToLower(), method);
                    }
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
                if (!CommandMethods.ContainsKey(command.CommandName))
                {
                    throw new Exception("No command method found for \"" + command.CommandName + "\"");
                }

                var method = CommandMethods[command.CommandName];

                try
                {
                    input = (string)method.Invoke(this, new object[] { input, command });
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

        public void AddCommand(string commandName, string defaultCommandArg, string variableName = null)
        {
            var command = new TextFilterCommand(commandName);
            command.CommandArgs = new Dictionary<string, string>() { { "default", defaultCommandArg } };
            command.VariableName = variableName;
            pipeline.Add(command);
        }

        public void AddCommands(IEnumerable<TextFilterCommand> commands)
        {
            pipeline.AddRange(commands);
        }

        public void AddCommands(IEnumerable<string> commandLines)
        {
            foreach (var line in commandLines)
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


        public void AddCommand(string commandName, Dictionary<string, string> commandArgs = null, string variableName = null)
        {
            var command = new TextFilterCommand(commandName);
            command.CommandArgs = commandArgs;
            command.VariableName = variableName;
            pipeline.Add(command);
        }

        [TextFilter("replace")]
        public string Replace(string input, TextFilterCommand command)
        {
            return input.Replace(command.CommandArgs["1"], command.CommandArgs["2"]);
        }

        [TextFilter("replaceall")]
        public string ReplaceAll(string input, TextFilterCommand command)
        {
            return command.DefaultArgument;
        }

        [TextFilter("format")]
        public string Format(string input, TextFilterCommand command)
        {
            var template = command.CommandArgs.ContainsKey("1") ? command.CommandArgs["1"] : String.Empty;
            template = command.CommandArgs.ContainsKey("template") && !String.IsNullOrWhiteSpace(command.CommandArgs["template"]) ? command.CommandArgs["template"] : template;

            return String.Format(template, input);
        }

        [TextFilter("extractfromhtml")]
        public string ExtractFromHtml(string input, TextFilterCommand command)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(input);

            var node = doc.DocumentNode.SelectSingleNode(command.CommandArgs["1"]);

            return node != null ? node.InnerHtml : String.Empty;
        }

        [TextFilter("extractfromxml")]
        public string ExtractFromXml(string input, TextFilterCommand command)
        {
            var doc = new XmlDocument();
            doc.LoadXml(input);

            var node = doc.DocumentElement.SelectSingleNode(command.CommandArgs["1"]);

            return node != null ? node.Value : String.Empty;
        }

        [TextFilter("transformxmlfromtemplate")]
        public string TransformXml(string input, TextFilterCommand command)
        {
            var arguments = new XsltArgumentList();
            arguments.AddExtensionObject("http://extensions", new XslExtensionObject());


            // We're going to do this transform using the templae as XSL
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(input);

            var transform = new XslCompiledTransform();
            transform.Load(XmlReader.Create(new StringReader(command.CommandArgs["template"])));

            var writer = new StringWriter();
            transform.Transform(xmlDoc, arguments, writer);
            return writer.ToString();
        }

        [TextFilter("ExtractFromJson")]
        public string ExtractFromJson(string input, TextFilterCommand command)
        {
            var jobject = (JToken)JObject.Parse(input);
            
            foreach (var segment in command.CommandArgs["1"].Split('.'))
            {
                if (segment[0] == '~')
                {
                    jobject = jobject[Convert.ToInt32(segment.Replace("~", String.Empty))];
                    continue;
                }

                if (jobject[segment] is JValue)
                {
                    return jobject[segment].ToString();
                }

                jobject = jobject[segment];
            }

            return String.Empty;
        }

        [TextFilter("addquerystringarg")]
        public string AddQuerysgtringArg(string input, TextFilterCommand command)
        {
            var uriBuilder = new UriBuilder(input);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[command.CommandArgs["1"]] = command.CommandArgs["2"];
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri.AbsoluteUri;
        }

        [TextFilter("append")]
        public string Append(string input, TextFilterCommand command)
        {
            return String.Concat(input, command.DefaultArgument);
        }

        [TextFilter("prepend")]
        public string Prepend(string input, TextFilterCommand command)
        {
            return String.Concat(command.DefaultArgument, input);
        }

    }


    public class XslExtensionObject
    {
        public string DateFormat(string input, string format)
        {
            return DateTime.Parse(input).ToLocalTime().ToString(format);
        }

        // This was really just for a demo...
        public string KToF(string input)
        {
            var kelvin = Double.Parse(input);

            return (((kelvin - 273.15) * 1.8)+32).ToString("###");

        }
    }
}