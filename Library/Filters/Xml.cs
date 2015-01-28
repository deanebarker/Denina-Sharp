using System;
using System.IO;
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

        [TextFilter("TransformXmlFromVariable", "Transforms an XML document against an XSL stylesheet")]
        [ArgumentMeta(1, "XSLT", true, "The raw XSLT to transform the input string.")]
        public static string TransformXml(string input, TextFilterCommand command)
        {
            var arguments = new XsltArgumentList();
            
            // We're going to do this transform using the templae as XSL
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(input);

            var transform = new XslCompiledTransform();
            transform.Load(XmlReader.Create(new StringReader(command.Pipeline.GetVariable(command.CommandArgs[0]).ToString())));

            var writer = new StringWriter();
            transform.Transform(xmlDoc, arguments, writer);
            return writer.ToString();
        }
    }
}