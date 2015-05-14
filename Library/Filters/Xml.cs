using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Xsl;
using BlendInteractive;
using BlendInteractive.Denina.Core.Documentation;
using DeninaSharp.Core.Documentation;
using DeninaSharp.Core.Filters;
using DeninaSharp.Core.Utility;

namespace DeninaSharp.Core.Filters
{
    [Filters("XML", "Working with XML strings.")]
    public class Xml
    {
        public static string XSLT_ARGUMENT_VARIABLE_NAME = "Xml.Transform.XsltExtensionClass";

        [Filter("Extract", "Extracts a single value from an XML document parsed from the input string.")]
        [ArgumentMeta("xpath", true, "The XPath identifying the desired XML node. The InnerText of the resulting node will be returned.")]
        [CodeSample("&lt;person&gt;&lt;name&gt;James Bond&lt;/name&gt;&lt;/person&gt;", "Xml.Extract //name", "James Bond")]
        public static string ExtractFromXml(string input, PipelineCommand command)
        {
            var doc = new XmlDocument();
            doc.LoadXml(input);

            var xpath = command.GetArgument("xpath");

            var node = doc.DocumentElement.SelectSingleNode(xpath);

            if (node == null)
            {
                return String.Empty;
            }

            return node is XmlElement ? node.InnerText : node.Value;
        }

        [Filter("Transform", "Transforms an XML document against an XSL stylesheet")]
        [ArgumentMeta("xslt", true, "The raw XSLT to transform the input string.")]
        [ArgumentMeta("xml", false, "The XML to transform.  If not provided, the XML is formed from the active text.")]
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
        public static string Transform(string input, PipelineCommand command)
        {
            var xml = String.Empty;
            var xsl = command.GetArgument("xslt");

            // This adds an extension object for XSL transforms
            var arguments = new XsltArgumentList();
            arguments.AddExtensionObject("http://denina", new XsltExtensions());

            // Do we want to pass in a custom extension object?
            if (Pipeline.IsSetGlobally(XSLT_ARGUMENT_VARIABLE_NAME))
            {
                var namespaceName = "ext";
                var className = Pipeline.GetGlobalVariable(XSLT_ARGUMENT_VARIABLE_NAME).ToString();

                // If they want to specify a custom namespace via "=", split and reassign the namespace and class names.
                if(className.Contains('='))
                {
                    namespaceName = className.Split('=').First();
                    className = className.Split('=').Last();
                }

                // Try to get the object
                ObjectHandle extensionObject;
                try
                {
                    extensionObject = Activator.CreateInstance(className.Split(',').Last(), className.Split(',').First());
                }
                catch (Exception e)
                {
                    throw new DeninaException(String.Format("Unable to load XsltExtension object \"{0}\"", className), e);
                }
                
                arguments.AddExtensionObject(String.Concat("http://", namespaceName), extensionObject.Unwrap());
            }

            // If there are two arguments, assume the second is XML
            if (command.HasArgument("xml"))
            {
                xml = command.GetArgument("xml");
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
            XmlReader xmlReader;
            try
            {
                xmlReader = XmlReader.Create(new StringReader(xml), settings);
            }
            catch (Exception e)
            {
                throw new DeninaException("Error parsing XML.", e);
            }

            // Form the XSL from the first argument
            XslCompiledTransform transform;
            try
            {
                transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(xsl), settings));

            }
            catch (Exception e)
            {
                throw new DeninaException("Error parsing XSL.", e);
            }

            // Do the transform (we're passing in an empty XsltArgumentList as a placeholder, in case we want to do something with it later...)
            var writer = new StringWriter();

            try
            {
                transform.Transform(xmlReader, arguments, writer);
            }
            catch (Exception e)
            {
                throw new DeninaException("Error performing XSLT transform.", e);
            }
            
            return writer.ToString().Replace("\u00A0", " ");    // This is a bit of a hack. We're replacing NO BREAK SPACE with a regular space. There has to be a way to fix this in the XSLT output.
        }

        [Filter("FormatNodes", "Performs token replacement on each node of a specified XPAth and returns the concatenated result.")]
        [ArgumentMeta("xpath", true, "XPath to return a list of XML nodes.")]
        [ArgumentMeta("template", true, "The template to apply to each node. XPath can be enclosed in brackets. These XPath expessions will be executed on the XML of that node and the resulting content will replace the token.")]
        [CodeSample(
            "&lt;rows&gt;\n&lt;row&gt;\n&lt;name&gt;James&lt;/name&gt;\n&lt;/row&gt;\n&lt;row&gt;\n&lt;name&gt;Bond&lt;/name&gt;\n&lt;/row&gt;\n&lt;/rows&gt;",
            "Xml.FormatNodes //row \"&lt;p&gt;Name: {name}&lt;/p&gt;\"",
            "&lt;p&gt;Name: James&lt;/p&gt;&lt;p&gt;Name: Bond&lt;/p&gt;"
            )]
        public static string FormatNodes(string input, PipelineCommand command)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(input);

            var xpath = command.GetArgument("xpath");
            var template = command.GetArgument("template");

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

        [Filter("CountNodes", "Returns the number of matching nodes.")]
        [ArgumentMeta("xpath", true, "XPath to return a list of XML nodes.")]
        [CodeSample(
            "&lt;rows&gt;\n&lt;row&gt;\n&lt;name&gt;James&lt;/name&gt;\n&lt;/row&gt;\n&lt;row&gt;\n&lt;name&gt;Bond&lt;/name&gt;\n&lt;/row&gt;\n&lt;/rows&gt;",
            "Xml.CountNodes //name",
            "2"
            )]
        public static string CountNodes(string input, PipelineCommand command)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(input);

            var xpath = command.CommandArgs.First().Value;

            if (xmlDoc.SelectNodes(xpath) == null)
            {
                return "0";
            }

            return xmlDoc.SelectNodes(xpath).Count.ToString();
        }
    }

}
