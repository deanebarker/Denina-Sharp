using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace BlendInteractive.TextFilterPipeline.Core.Filters
{
    [TextFilters("XML")]
    public class Xml
    {
        [TextFilter("Extract")]
        public static string ExtractFromXml(string input, TextFilterCommand command)
        {
            var doc = new XmlDocument();
            doc.LoadXml(input);

            XmlNode node = doc.DocumentElement.SelectSingleNode(command.CommandArgs[0]);

            return node != null ? node.Value : String.Empty;
        }

        [TextFilter("TransformXmlFromVariable")]
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