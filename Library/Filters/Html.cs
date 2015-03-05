using System;
using System.IO;
using System.Web.UI;
using BlendInteractive.Denina.Core.Documentation;
using DeninaSharp.Core.Documentation;
using HtmlAgilityPack;

namespace DeninaSharp.Core.Filters
{
    [Filters("HTML", "Creating and manipulating HTML strings.")]
    public static class Html
    {
        [Filter("Extract", "Extracts an element from an HTML string. (Relies on the HtmlAgilityPack.dll assembly, which must be available.)")]
        [ArgumentMeta(1, "Path", true, "The XPath(-ish) to the element, based on HtmlAgilityPack's language standard.")]
        [CodeSample("<html><body><div id=\"a\">James Bond</div></body></html>", "Html.Extract //div[@id='a']", "James Bond")]
        public static string Extract(string input, PipelineCommand command)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(input);

            HtmlNode node = doc.DocumentNode.SelectSingleNode(command.CommandArgs[0]);

            return node != null ? node.InnerHtml : String.Empty;
        }

        [Filter("Wrap", "Wraps the input string in a specified HTML tag with optional class and/or ID.")]
        [ArgumentMeta(1, "Tag Name", true, "The name of the HTML tag in which to wrap the content.")]
        [ArgumentMeta(2, "Class", false, "If provided, the tag will use this as a \"class\" attribute.")]
        [ArgumentMeta(3, "ID", false, "If provided, the tag will use this as an \"id\" attribute.")]
        [CodeSample("James Bond", "Html.Wrap div spy agent", "&lt;div id=\"spy\" class=\"agent\"&gt;James Bond&lt;/div&gt;")]
        public static string WrapInTag(string input, PipelineCommand command)
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

        [Filter("LineBreaks", "Replaces line breaks with the corresponding HTML tag.")]
        [CodeSample("James\nBond", "Html.LineBreaks", "James&lt;br/&gt;Bond")]
        public static string LineBreaks(string input, PipelineCommand command)
        {
            return input.Replace(Environment.NewLine, "<br/>");
        }
    }
}