using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using BlendInteractive.Denina.Core.Documentation;
using DeninaSharp.Core;
using DeninaSharp.Core.Documentation;

namespace Documentor
{
    class Program
    {
        private const string template = @"
            <html>
                <head>
                    <style>
                        body, td, th {{ font-family: 'Helvetica Neue', Helvetica; font-size: 16px; line-height: 1.6; }}
                        td, th {{ vertical-align: top; border-bottom: solid 1px rgb(230,230,230); text-align: left; padding: 5px; padding-right: 15px;}}
                        th {{ background-color: rgb(240,240,240); }}
                        table {{ border-collapse: collapse;}}
                        h3 {{ margin-top: 30px; margin-bottom: 3px; }}
                        h2 {{ margin-top: 40px; background-color: rgb(240,240,240); padding: 10px; width: 100%; }}
                        h4 {{ margin-bottom: 5px; }}
                        p {{ margin: 0 0 1em 0; }}
                        div.filter-meta {{ margin-left: 0px; }}
                        .argument-name, .required {{ white-space: nowrap; vertical-align: top; }}
                        div#container {{ width: 790px; margin: auto; }}
                        table {{ width: 100%; }}
                        h1 {{ padding-bottom: 0.3em; font-size: 2.25em; line-height: 1.2; border-bottom: 1px solid #eee; }}
                        body {{padding-bottom: 100px; }} 
                        .top-link {{ display: block; margin-top: 20px; }}  
                        code {{ display: block; font-size: 13px; font-family: courier new; line-height: 18px; padding: 4px; background-color: rgb(80,80,80); color: white; }}     
                        table.code-samples {{ margin-bottom: 1em; border-top: solid 1px rgb(230,230,230); }}        
                        table.code-samples th {{ width: 100px; }}
                    </style>
                </head>
                <body>
                <a name=""top""></a>
                <div id=""container"">{0}</div></body>
            </html>
            ";

        static void Main(string[] args)
        {
            var stringWriter = new StringWriter();

            var settings = new XmlWriterSettings()
            {
                Encoding = Encoding.Unicode,
                OmitXmlDeclaration = true,
                Indent = true
            };
                

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var xmlWriter = new XmlTextWriter(stringWriter);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("root");

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof (FiltersAttribute), true).Any()))
                {
                    // Don't write the test filters
                    if (type.Name == "Test")
                    {
                        continue;
                    }
                    
                    xmlWriter.WriteStartElement("category");

                    var serializer1 = new XmlSerializer(typeof(FiltersAttribute));
                    serializer1.Serialize(xmlWriter, type.GetCustomAttributes(typeof(FiltersAttribute), true).First(), ns);

                    foreach (MethodInfo method in type.GetMethods().Where(m => m.GetCustomAttributes(typeof (FilterAttribute), true).Any()))
                    {
                        xmlWriter.WriteStartElement("filter");
                        
                        var serializer2 = new XmlSerializer(typeof(FilterAttribute));
                        serializer2.Serialize(xmlWriter, method.GetCustomAttributes(typeof(FilterAttribute), true).First(), ns);

                        if (method.GetCustomAttributes(typeof (ArgumentMetaAttribute), true).Any())
                        {

                            xmlWriter.WriteStartElement("arguments");

                            foreach (ArgumentMetaAttribute attribute in method.GetCustomAttributes(typeof (ArgumentMetaAttribute), true))
                            {
                                var serializer3 = new XmlSerializer(typeof (ArgumentMetaAttribute));
                                serializer3.Serialize(xmlWriter, attribute, ns);
                            }

                            xmlWriter.WriteEndElement();
                        }

                        if (method.GetCustomAttributes(typeof(RequiresAttribute), true).Any())
                        {

                            xmlWriter.WriteStartElement("dependencies");

                            foreach (RequiresAttribute attribute in method.GetCustomAttributes(typeof(RequiresAttribute), true))
                            {
                                var serializer3 = new XmlSerializer(typeof(RequiresAttribute));
                                serializer3.Serialize(xmlWriter, attribute, ns);
                            }

                            xmlWriter.WriteEndElement();
                        }

                        if (method.GetCustomAttributes(typeof(CodeSampleAttribute), true).Any())
                        {

                            xmlWriter.WriteStartElement("samples");

                            foreach (CodeSampleAttribute attribute in method.GetCustomAttributes(typeof(CodeSampleAttribute), true))
                            {
                                var serializer3 = new XmlSerializer(typeof(CodeSampleAttribute));
                                serializer3.Serialize(xmlWriter, attribute, ns);
                            }

                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                }

            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();


            var xml = stringWriter.ToString();

            var categoryXslt = new XslCompiledTransform();
            categoryXslt.Load(XmlReader.Create(new StringReader(File.ReadAllText("category.xslt"))));

            var homeXslt = new XslCompiledTransform();
            homeXslt.Load(XmlReader.Create(new StringReader(File.ReadAllText("home.xslt"))));

            var extenstionObject = new XsltExtensions();
            var arguments = new XsltArgumentList();
            arguments.AddExtensionObject("http://denina", extenstionObject);

            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var homeFile = File.OpenWrite("index.html");
            homeXslt.Transform(XmlReader.Create(new StringReader(xml), new XmlReaderSettings()), arguments, homeFile);
            homeFile.Close();

            foreach(XmlElement category in doc.SelectNodes("//category"))
            {
                var fileName = extenstionObject.CleanFileName(category.SelectSingleNode("categoryMeta/Category").InnerText);

                var categoryFile = File.OpenWrite(fileName + ".html");
                categoryXslt.Transform(XmlReader.Create(new StringReader(category.OuterXml)), arguments, categoryFile);
                categoryFile.Close();
            }

            //File.WriteAllText("doc.html", String.Format(template, writer.ToString()));

        }
        

        /*static void Main2(string[] args)
        {
            

            var stringWriter = new StringWriter();
            var html = new HtmlTextWriter(stringWriter);

            html.RenderBeginTag("h1");
            html.Write("Available Filters");
            html.RenderEndTag();

            IEnumerable<Type> types = Pipeline.Types.OrderBy(t => t.Key).Select(t => t.Value);


            html.RenderBeginTag("ul");
            foreach (Type type in types)
            {
                var textFiltersAttribute = (FiltersAttribute) type.GetCustomAttributes(typeof (FiltersAttribute), true).First();
                html.RenderBeginTag("li");
                html.AddAttribute("href", "#" + textFiltersAttribute.Category);
                html.RenderBeginTag("a");
                html.Write(textFiltersAttribute.Category);
                html.RenderEndTag();
                html.RenderEndTag();
            }
            html.RenderEndTag();

            foreach (Type textFilterCategory in Pipeline.Types.OrderBy(t => t.Key).Select(t => t.Value))
            {
                var textFiltersAttribute = (FiltersAttribute) textFilterCategory.GetCustomAttributes(typeof (FiltersAttribute), true).First();

                html.AddAttribute("name", textFiltersAttribute.Category);
                html.RenderBeginTag("a");
                html.RenderEndTag();

                html.RenderBeginTag("h2");
                html.Write(textFiltersAttribute.Category);
                html.RenderEndTag();


                if (!String.IsNullOrWhiteSpace(textFiltersAttribute.Description))
                {
                    html.RenderBeginTag("p");
                    html.Write(textFiltersAttribute.Description);
                    html.RenderEndTag();
                }

                foreach (MethodInfo method in textFilterCategory.GetMethods().Where(m => m.GetCustomAttributes(typeof (FilterAttribute), true).Any()))
                {
                    var textFilterAttribute = (FilterAttribute) method.GetCustomAttributes(typeof (FilterAttribute), true).First();

                    html.AddAttribute("class", "filter-meta");
                    html.RenderBeginTag("div");

                    html.RenderBeginTag("h3");
                    html.Write(textFilterAttribute.Name);
                    html.RenderEndTag();

                    if (!String.IsNullOrWhiteSpace(textFilterAttribute.Description))
                    {
                        html.RenderBeginTag("p");
                        html.Write(textFilterAttribute.Description);
                        html.RenderEndTag();
                    }

                    IEnumerable<ArgumentMetaAttribute> helpAttributes = method.GetCustomAttributes(typeof (ArgumentMetaAttribute), true).Select(a => (ArgumentMetaAttribute) a);
                    if (helpAttributes.Any())
                    {
                        html.RenderBeginTag("table");
                        html.RenderBeginTag("tr");
                        html.AddAttribute("class", "argument-name");
                        html.RenderBeginTag("th");
                        html.Write("Argument");
                        html.RenderEndTag();
                        html.AddAttribute("class", "required");
                        html.RenderBeginTag("th");
                        html.Write("Required");
                        html.RenderEndTag();
                        html.RenderBeginTag("th");
                        html.Write("Description");
                        html.RenderEndTag();
                        html.RenderEndTag();


                        foreach (ArgumentMetaAttribute argument in helpAttributes.OrderBy(a => a.Ordinal))
                        {
                            html.RenderBeginTag("tr");
                            html.AddAttribute("class", "argument-name");
                            html.RenderBeginTag("td");
                            html.Write(argument.Name);
                            html.RenderEndTag();
                            html.AddAttribute("class", "required");
                            html.RenderBeginTag("td");
                            html.Write(argument.Required.ToString());
                            html.RenderEndTag();
                            html.RenderBeginTag("td");
                            html.Write(argument.Description);
                            html.RenderEndTag();
                            html.RenderEndTag();
                        }

                        html.RenderEndTag();
                    }
                    else
                    {
                        html.RenderBeginTag("p");
                        html.RenderBeginTag("i");
                        html.Write("Takes no arguments.");
                        html.RenderEndTag();
                        html.RenderEndTag();
                    }

                    html.RenderEndTag();
                }


                html.AddAttribute("href", "#top");
                html.AddAttribute("class", "top-link");
                html.RenderBeginTag("a");
                html.Write("Back to Top");
                html.RenderEndTag();
            }

            return String.Format(template, stringWriter);
        }*/
    }
}
