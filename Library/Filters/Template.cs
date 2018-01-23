using DeninaSharp.Core;
using DeninaSharp.Core.Documentation;
using DotLiquid;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace BlendInteractive.Denina.Core.Filters
{
    [Filters("Template", "Templating output with DotLiquid.")]
    public static class Template
    {
        [Filter("FromText", "Processes a DotLiquid template with the input text as the 'data' variable.")]
        [Requires("DotLiquid.Template, DotLiquid", "DotLiquid is an open-source templating library.")]
        [ArgumentMeta("template", true, "A DotLiquid template string.")]
        public static string FromText(string input, PipelineCommand command)
        {
            var template = command.GetArgument("template");

            var parsedTemplate = DotLiquid.Template.Parse(template);
            return parsedTemplate.Render(Hash.FromAnonymousObject(
                    new { data = input, vars = new Drops.VarsDrop(command.Pipeline.Variables) }
                ));
        }

        [Filter("FromXml", "Processes a DotLiquid template with the 'data' variable representing either the entire input XML, or a collection of XML nodes.")]
        [Requires("DotLiquid.Template, DotLiquid", "DotLiquid is an open-source templating library.")]
        [ArgumentMeta("template", true, "A DotLiquid template string.")]
        [ArgumentMeta("xpath", false, "If supplied, the XPath will be executed on the XML to return a node list. The 'data' variable in the DotLiquid template will be a collection of these nodes. If not supplied, the 'data variable will be the entire XML as a single object.")]
        public static string FromXml(string input, PipelineCommand command)
        {
            var xpath = command.GetArgument("xpath", string.Empty);
            var template = command.GetArgument("template");

            var parsedTemplate = DotLiquid.Template.Parse(template);

            // Parse the incoming XML
            var xml = new XmlDocument();
            try
            {
                xml.LoadXml(input);
            }
            catch (Exception e)
            {
                throw new DeninaException("XML Not Valid", e);
            }

            if (!String.IsNullOrWhiteSpace(xpath))
            {
                // If they supplied XPath, then we need to run it against the input to find multiple, individual XML nodes
                // We feed those to the template as a collection
                // "data" in the template is a List<XmlNode>

                // Load the XML nodes into a list of objects
                var templateData = new List<Drops.XmlNode>();
                foreach (XmlElement node in xml.SelectNodes(xpath))
                {
                    templateData.Add(new Drops.XmlNode(node.OuterXml));
                }

                return parsedTemplate.Render(Hash.FromAnonymousObject(
                    new { data = templateData, vars = new Drops.VarsDrop(command.Pipeline.Variables) }
                ));
            }
            else
            {
                // No xpath, so the entire input is feed to the template as a single object
                // "data" in the template is a single XmlNode
                return parsedTemplate.Render(Hash.FromAnonymousObject(
                    new { data = new Drops.XmlNode(xml), vars = new Drops.VarsDrop(command.Pipeline.Variables) }
                ));
            }
        }

        public static class Drops
        {

            public class VarsDrop : Drop
            {
                private Dictionary<string, string> vars;

                public VarsDrop(IDictionary<string, PipelineVariable> vars)
                {
                    this.vars = vars.ToDictionary(v => v.Key, v => v.Value.Value.ToString());
                }
                public override object BeforeMethod(string method)
                {
                    return vars[method] ?? string.Empty;
                }
            }

            // Represents an XML document
            // Allows addressing content in a DotLiquid template
            // ex: "data.person.name.first" to find the text in the XML node at "/person/name/first"
            public class XmlNode : Drop
            {
                private const char attributeIndicatorChar = '_';
                private const string xpathQueryMethodName = "xpath";

                private XmlElement doc;

                public XmlNode(XmlDocument xml)
                {
                    doc = xml.DocumentElement;
                }

                public XmlNode(XmlElement xml)
                {
                    doc = xml;
                }

                public XmlNode(string xml)
                {
                    var xmlDoc = new XmlDocument();
                    try
                    {
                        xmlDoc.LoadXml(xml);
                    }
                    catch (XmlException e) when (e.Message.Contains("multiple root elements"))
                    {
                        xmlDoc.LoadXml($"<root>{xml}</root>");
                    }
                    doc = xmlDoc.DocumentElement;
                }

                private string GetPathToNode(string path)
                {
                    // Yes, this just returns the input
                    // It might change in the future, hence the abstraction
                    return $"{path}";
                }

                private string GetPathToTextNode(string path)
                {
                    return $"{path}/text()";
                }

                public override object BeforeMethod(string method)
                {
                    if (method == xpathQueryMethodName)
                    {
                        return new XPathQuery(doc);
                    }

                    if (method[0] == attributeIndicatorChar)
                    {
                        // Attribute selector
                        // If it starts with an underscore, swap for an "@"
                        // (You can't start DotLiquid methods with a "@", so we have to hack with the underscore.)
                        method = string.Concat("@", method.Substring(1));
                    }

                    if (doc.SelectSingleNode(GetPathToNode(method)) == null)
                    {
                        // We have nothing for this node, return an empty string
                        return string.Empty;
                    }

                    if (doc.SelectSingleNode(GetPathToTextNode(method)) != null)
                    {
                        // We have inner text for this node, return it
                        return doc.SelectSingleNode(GetPathToTextNode(method)).InnerText;
                    }

                    // We have a new node for this method, so return a new node so they can drill down further
                    return new XmlNode((XmlElement)doc.SelectSingleNode(GetPathToNode(method)));
                }
            }

            public class XPathQuery : Drop, IIndexable
            {
                private XmlElement doc;

                public XPathQuery(XmlElement xml)
                {
                    doc = xml;
                }

                public override object this[object key] => doc.SelectSingleNode(key.ToString())?.InnerText;
                public override bool ContainsKey(object name) => true;
            }
        }
    }
}