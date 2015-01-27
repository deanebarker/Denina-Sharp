using System;
using System.IO;
using System.Web.UI;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace BlendInteractive.TextFilterPipeline.Core.Filters
{
    [TextFilters("HTML")]
    public static class Html
    {
        [TextFilter("Extract")]
        public static string Extract(string input, TextFilterCommand command)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(input);

            HtmlNode node = doc.DocumentNode.SelectSingleNode(command.CommandArgs[0]);
            if (node == null) { 
                node = doc.DocumentNode.QuerySelector(command.CommandArgs[0]);
            }

            return node != null ? node.InnerHtml : String.Empty;
        }

        [TextFilter("Wrap")]
        public static string WrapInTag(string input, TextFilterCommand command)
        {
            var stringWriter = new StringWriter();
            var tagBuilder = new HtmlTextWriter(stringWriter);

            // The second argument should be the class
            if (command.CommandArgs.ContainsKey(1))
            {
                tagBuilder.AddAttribute("class", command.CommandArgs[1]);
            }

            // The third argument should be the id
            if (command.CommandArgs.ContainsKey(2))
            {
                tagBuilder.AddAttribute("id", command.CommandArgs[2]);
            }

            tagBuilder.RenderBeginTag(command.CommandArgs[0]);
            tagBuilder.Write(input);
            tagBuilder.RenderEndTag();

            return stringWriter.ToString();
        }
    }
}