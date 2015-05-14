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
        [ArgumentMeta("xpath", true, "The XPath(-ish) to the element, based on HtmlAgilityPack's language standard.")]
        [CodeSample("<html><body><div id=\"a\">James Bond</div></body></html>", "Html.Extract //div[@id='a']", "James Bond")]
        [Requires("HtmlAgilityPack.HtmlDocument, HtmlAgilityPack", "HtmlAgilityPack is an open-source HTML parsing library.")]
        public static string Extract(string input, PipelineCommand command)
        {
            var xpath = command.GetArgument("xpath");

            var doc = new HtmlDocument();
            doc.LoadHtml(input);

            HtmlNode node = doc.DocumentNode.SelectSingleNode(xpath);

            return node != null ? node.InnerHtml : String.Empty;
        }

        [Filter("Wrap", "Wraps the input string in a specified HTML tag with optional class and/or ID.")]
        [ArgumentMeta("tag", true, "The name of the HTML tag in which to wrap the content.")]
        [ArgumentMeta("class", false, "If provided, the tag will use this as a \"class\" attribute.")]
        [ArgumentMeta("id", false, "If provided, the tag will use this as an \"id\" attribute.")]
        [CodeSample("James Bond", "Html.Wrap div spy agent", "&lt;div id=\"spy\" class=\"agent\"&gt;James Bond&lt;/div&gt;")]
        public static string WrapInTag(string input, PipelineCommand command)
        {
            var stringWriter = new StringWriter();
            var tagBuilder = new HtmlTextWriter(stringWriter);

            // The second argument should be the class
            if (command.HasArgument("class"))
            {
                tagBuilder.AddAttribute("class", command.GetArgument("class"));
            }

            // The third argument should be the id
            if (command.HasArgument("id"))
            {
                tagBuilder.AddAttribute("id", command.GetArgument("id"));
            }

            tagBuilder.RenderBeginTag(command.GetArgument("tag"));
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