using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Xsl;
using BlendInteractive.TextFilterPipeline.Core.Documentation;

namespace BlendInteractive.TextFilterPipeline.Core.Filters
{
    [TextFilters("XML", "Working with XML strings.")]
    public class Xml
    {
        [TextFilter("Extract", "Extracts a single value from an XML document parsed from the input string.")]
        [ArgumentMeta(1, "XPath", true, "The XPath identifying the desired XML node. The InnerText of the resulting node will be returned.")]
        public static string ExtractFromXml(string input, TextFilterCommand command)
        {
            var doc = new XmlDocument();
            doc.LoadXml(input);

            XmlNode node = doc.DocumentElement.SelectSingleNode(command.CommandArgs[0]);

            return node != null ? node.Value : String.Empty;
        }

        [TextFilter("TransformXml", "Transforms an XML document against an XSL stylesheet")]
        [ArgumentMeta(1, "XSLT", true, "The raw XSLT to transform the input string.")]
        [ArgumentMeta(2, "XML", false, "The XML to transform.  If not provided, the XML is formed from the active text.")]
        public static string TransformXml(string input, TextFilterCommand command)
        {
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
            
            // Form the XML doc from the input
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            // Form the XSL from the first argument
            var transform = new XslCompiledTransform();
            transform.Load(XmlReader.Create(new StringReader(xsl)));

            // Do the transform (we're passing in an empty XsltArgumentList as a placeholder, in case we want to do something with it later...)
            var writer = new StringWriter();
            transform.Transform(xmlDoc, new XsltArgumentList(), writer);
            return writer.ToString();
        }
    }
}