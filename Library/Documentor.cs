using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using BlendInteractive.TextFilterPipeline.Core.Documentation;

namespace BlendInteractive.TextFilterPipeline.Core
{
    public static class Documentor
    {



        public static string Generate()
        {
            var stringWriter = new StringWriter();
            var html = new HtmlTextWriter(stringWriter);

            html.RenderBeginTag("h1");
            html.Write("Available Filters");
            html.RenderEndTag();

            var types = TextFilterPipeline.Types.OrderBy(t => t.Key).Select(t => t.Value);


            html.RenderBeginTag("ul");
            foreach (var type in types)
            {
                var textFiltersAttribute = (TextFiltersAttribute) type.GetCustomAttributes(typeof (TextFiltersAttribute), true).First();
                html.RenderBeginTag("li");
                html.AddAttribute("href", "#" + textFiltersAttribute.Category);
                html.RenderBeginTag("a");
                html.Write(textFiltersAttribute.Category);
                html.RenderEndTag();
                html.RenderEndTag();
            }
            html.RenderEndTag();

            foreach (var textFilterCategory in TextFilterPipeline.Types.OrderBy(t => t.Key).Select(t => t.Value))
            {
                var textFiltersAttribute = (TextFiltersAttribute) textFilterCategory.GetCustomAttributes(typeof (TextFiltersAttribute), true).First();
                
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

                foreach (var method in textFilterCategory.GetMethods().Where(m => m.GetCustomAttributes(typeof(TextFilterAttribute), true).Any()))
                {
                    var textFilterAttribute = (TextFilterAttribute) method.GetCustomAttributes(typeof (TextFilterAttribute), true).First();
                    
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

                    var helpAttributes = method.GetCustomAttributes(typeof (ArgumentMetaAttribute), true).Select(a => (ArgumentMetaAttribute) a);
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
        }

        private const string template = @"
            <html>
                <head>
                    <style>
                        body, td, th {{ font-family: 'Helvetica Neue', Helvetica; font-size: 16px; line-height: 1.6; }}
                        td, th {{ border-bottom: solid 1px rgb(230,230,230); text-align: left; padding: 5px; padding-right: 15px;}}
                        th {{ background-color: rgb(240,240,240); }}
                        table {{ border-collapse: collapse;}}
                        h3 {{ margin-top: 30px; margin-bottom: 3px; }}
                        h2 {{ margin-top: 40px; background-color: rgb(240,240,240); padding: 10px; width: 100%; }}
                        p {{ margin: 0 0 1em 0; }}
                        div.filter-meta {{ margin-left: 0px; }}
                        .argument-name, .required {{ white-space: nowrap; vertical-align: top; }}
                        div#container {{ width: 790px; margin: auto; }}
                        table {{ width: 100%; }}
                        h1 {{ padding-bottom: 0.3em; font-size: 2.25em; line-height: 1.2; border-bottom: 1px solid #eee; }}
                        body {{padding-bottom: 100px; }} 
                        .top-link {{ display: block; margin-top: 20px; }}               
                    </style>
                </head>
                <body>
                <a name=""top""></a>
                <div id=""container"">{0}</div></body>
            </html>
            ";
    }
}
