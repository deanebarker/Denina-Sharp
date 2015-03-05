using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Xsl;
using BlendInteractive.Denina.Core.Documentation;
using DeninaSharp.Core.Documentation;
using DeninaSharp.Core.Filters;

namespace DeninaSharp.Core.Filters
{
    [Filters("XML", "Working with XML strings.")]
    public class Xml
    {
        public static string XSLT_ARGUMENT_VARIABLE_NAME = "__xsltarguments";

        [Filter("Extract", "Extracts a single value from an XML document parsed from the input string.")]
        [ArgumentMeta(1, "XPath", true, "The XPath identifying the desired XML node. The InnerText of the resulting node will be returned.")]
        [CodeSample("<person><name>James Bond</name></person>", "Xml.Extract //name", "James Bond")]
        public static string ExtractFromXml(string input, PipelineCommand command)
        {
            var doc = new XmlDocument();
            doc.LoadXml(input);

            XmlNode node = doc.DocumentElement.SelectSingleNode(command.CommandArgs[0]);

            return node != null ? node.Value : String.Empty;
        }

        [Filter("Transform", "Transforms an XML document against an XSL stylesheet")]
        [ArgumentMeta(1, "XSLT", true, "The raw XSLT to transform the input string.")]
        [ArgumentMeta(2, "XML", false, "The XML to transform.  If not provided, the XML is formed from the active text.")]
        [CodeSample(
            "",
            @"File.Read xml-file.xml => $xml
            File.Read xslt-file.xslt => $xslt
            Xml.Transform $xslt $xml",
            "(The transformed XML)"
            )]
        [CodeSample(
            "(An XML string)",
            @"File.Read xslt-file.xslt => $xslt
            Xml.Transform $xslt",
            "(The transformed XML)"
            )]
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

        [Filter("FormatNodes", "Performs token replacement on each node of a specified XPAth and returns the concatenated result.")]
        [ArgumentMeta(1, "XPath", true, "XPath to return a list of XML nodes.")]
        [ArgumentMeta(2, "Template", true, "The template to apply to each node. XPath can be enclosed in brackets. These XPath expessions will be executed on the XML of that node and the resulting content will replace the token.")]
        [CodeSample(
            "&lt;rows&gt;\n&lt;row&gt;\n&lt;name&gt;James&lt;/name&gt;\n&lt;/row&gt;\n&lt;row&gt;\n&lt;name&gt;Bond&lt;/name&gt;\n&lt;/row&gt;\n&lt;/rows&gt;",
            "Xml.FormatNodes //row \"&lt;p&gt;Name: {name}&lt;/p&gt;\"",
            "&lt;p&gt;Name: James&lt;/p&gt;&lt;p&gt;Name: Bond&lt;/p&gt;"
            )]
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

namespace DeninaSharp.Core
{
    public partial class Pipeline
    {
        public static void AddXsltArgumentList(object argumentList)
        {
            SetGlobalVariable(Xml.XSLT_ARGUMENT_VARIABLE_NAME, argumentList);
        }
    }
}