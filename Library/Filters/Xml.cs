using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;
using BlendInteractive.Denina.Core.Documentation;
using BlendInteractive.Denina.Core.Filters;

namespace BlendInteractive.Denina.Core.Filters
{
    [Filters("XML", "Working with XML strings.")]
    public class Xml
    {
        public static string XSLT_ARGUMENT_VARIABLE_NAME = "__xsltarguments";

        [Filter("Extract", "Extracts a single value from an XML document parsed from the input string.")]
        [ArgumentMeta(1, "XPath", true, "The XPath identifying the desired XML node. The InnerText of the resulting node will be returned.")]
        public static string ExtractFromXml(string input, PipelineCommand command)
        {
            var doc = new XmlDocument();
            doc.LoadXml(input);

            XmlNode node = doc.DocumentElement.SelectSingleNode(command.CommandArgs[0]);

            return node != null ? node.Value : String.Empty;
        }

        [Filter("TransformXml", "Transforms an XML document against an XSL stylesheet")]
        [ArgumentMeta(1, "XSLT", true, "The raw XSLT to transform the input string.")]
        [ArgumentMeta(2, "XML", false, "The XML to transform.  If not provided, the XML is formed from the active text.")]
        public static string TransformXml(string input, PipelineCommand command)
        {
            var arguments = Pipeline.IsSetGlobally(XSLT_ARGUMENT_VARIABLE_NAME) ? (XsltArgumentList) Pipeline.GetGlobalVariable(XSLT_ARGUMENT_VARIABLE_NAME) : new XsltArgumentList();

            var xml = String.Empty;
            var xsl = command.CommandArgs.First().Value;

            // If there are two arguments, assume the second is XML
            if (command.CommandArgs.Count == 2)
            {
                xml = command.CommandArgs[1];
            }
            else
            {
                // Otherwise, the XML is the input
                xml = input;
            }
            
            // Set some basic parameters for our XML reading...
            var settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true
            };
            
            // Form the XML doc from the input
            var xmlReader = XmlReader.Create(new StringReader(xml), settings);

            // Form the XSL from the first argument
            var transform = new XslCompiledTransform();
            transform.Load(XmlReader.Create(new StringReader(xsl), settings));

            // Do the transform (we're passing in an empty XsltArgumentList as a placeholder, in case we want to do something with it later...)
            var writer = new StringWriter();
            transform.Transform(xmlReader, arguments, writer);
            return writer.ToString().Replace("\u00A0", " ");    // This is a bit of a hack. We're replacing NO BREAK SPACE with a regular space. There has to be a way to fix this in the XSLT output.
        }

        [Filter("FormatNodes")]
        public static string FormatNodes(string input, PipelineCommand command)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(input);

            var xpath = command.CommandArgs.First().Value;
            var template = command.CommandArgs[1];

            var patterns = Regex.Matches(template, "{([^}]*)}").Cast<Match>().Select(m => m.Value).ToList();

            var output = new StringBuilder();
            foreach (XmlNode node in xmlDoc.SelectNodes(xpath))
            {
                var thisTemplate = String.Copy(template);
                foreach (var pattern in patterns)
                {
                    var value = node.SelectSingleNode(pattern.Trim(new char[] {'}', '{'})).InnerXml;
                    thisTemplate = thisTemplate.Replace(pattern, value);
                }
                output.Append(thisTemplate);
            }

            return output.ToString();
        }
    }

}

namespace BlendInteractive.Denina.Core
{
    public partial class Pipeline
    {
        public static void AddXsltArgumentList(object argumentList)
        {
            SetGlobalVariable(Xml.XSLT_ARGUMENT_VARIABLE_NAME, argumentList);
        }
    }
}