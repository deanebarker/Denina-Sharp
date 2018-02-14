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
        public static string FromXml(string input, PipelineCommand command)
        {
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

            return parsedTemplate.Render(Hash.FromAnonymousObject(
                new { data = new Drops.XmlNode(xml), vars = new Drops.VarsDrop(command.Pipeline.Variables) }
            ));
        }

        public static class Drops
        {

            public class VarsDrop : Drop
            {
                private Dictionary<string, string> vars;

                public VarsDrop(IDictionary<string, PipelineVariable> vars)
                {
                    this.vars = vars.ToDictionary(v => v.Key, v => v.Value.Value?.ToString() ?? string.Empty);
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
                    // If they want to run pure XPath, send them back a query object
                    if (method == xpathQueryMethodName)
                    {
                        return new XPathQuery(doc);
                    }

                    // Attribute selector
                    // If it starts with an underscore, swap for an "@"
                    // (You can't start DotLiquid methods with a "@", so we have to hack with the underscore.)
                    if (method[0] == attributeIndicatorChar)
                    {
                        method = string.Concat("@", method.Substring(1));
                    }

                    // We have nothing for this node, return an empty string
                    if (doc.SelectSingleNode(GetPathToNode(method)) == null)
                    {
                        return string.Empty;
                    }

                    // If there's more than one node, return a list of them
                    if(doc.SelectNodes(GetPathToNode(method)).Count > 1)
                    {
                        var drops = new List<XmlNode>();
                        foreach(XmlElement node in doc.SelectNodes(GetPathToNode(method)))
                        {
                            drops.Add(new XmlNode(node));
                        }
                        return drops;
                     }

                    // We have inner text for this node, then this is what they actually want, so return it
                    if (doc.SelectSingleNode(GetPathToTextNode(method)) != null)
                    {
                        return doc.SelectSingleNode(GetPathToTextNode(method)).InnerText;
                    }

                    // If there's no text, then assume they want to drill down into the inner XML of it
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