using AngleSharp.Parser.Html;
using BlendInteractive.Denina.Core;
using DeninaSharp.Core.Documentation;
using System;
using System.IO;
using System.Linq;
using System.Web.UI;

namespace DeninaSharp.Core.Filters
{
    [Filters("HTML", "Creating and manipulating HTML strings.")]
    public static class Html
    {
        [Filter("Extract", "Extracts an element from an HTML string.")]
        [ArgumentMeta("path", true, "The AngleSharp selector (very CSS like).")]
        [CodeSample("resource:html-extract-input.html", "Html.Extract -path:div#a", "James Bond")]
        [Requires("AngleSharp.Parser.Html.HtmlParser, AngleSharp", "AngleSharp is an open-source markup parsing library.")]
        public static string Extract(string input, PipelineCommand command, ExecutionLog log)
        {
            var parser = new HtmlParser(input);
            var doc = parser.Parse();
            var element = doc.QuerySelector(command.GetArgument("path"));

            return element == null ? String.Empty : element.InnerHtml;
        }

        [Filter("Strip", "Strips tags from the HTML string.)")]
        [CodeSample("James <b>Bond</b>.", "Html.StripTags", "James Bond.")]
        [Requires("AngleSharp.Parser.Html.HtmlParser, AngleSharp", "AngleSharp is an open-source markup parsing library.")]
        public static string StripTags(string input, PipelineCommand command, ExecutionLog log)
        {
            var parser = new HtmlParser(input);
            var doc = parser.Parse();
            return doc.DocumentElement.TextContent;
        }

        [Filter("Wrap", "Wraps the input string in a specified HTML tag with optional class and/or ID.")]
        [ArgumentMeta("tag", true, "The name of the HTML tag in which to wrap the content.")]
        [ArgumentMeta("class", false, "If provided, the tag will use this as a \"class\" attribute.")]
        [ArgumentMeta("id", false, "If provided, the tag will use this as an \"id\" attribute.")]
        [CodeSample("James Bond", "Html.Wrap -tag:div -class:spy -id:agent", "<div id=\"spy\" class=\"agent\">James Bond</div>")]
        public static string WrapInTag(string input, PipelineCommand command, ExecutionLog log)
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
        [CodeSample("James\nBond", "Html.LineBreaks", "James<br/>Bond")]
        public static string LineBreaks(string input, PipelineCommand command, ExecutionLog log)
        {
            return input.Replace(Environment.NewLine, "<br/>");
        }

        [Filter("MakeTag", "Creates an arbitrary HTML tag from supplied data.")]
        [ArgumentMeta("tag", true, "The name of the tag.")]
        [ArgumentMeta("content", false, "The content of the tag. If not supplied, the input text will be used.")]
        [ArgumentMeta("*", false, "All arguments other than \"tag\" and \"content\" will be converted to HTML attributes of the same name and value.")]
        [CodeSample("(None)", "Html.MakeTag -tag:div -content:\"James Bond\" -data-number:007", "<div data-number=\"007\">James Bond</div>")]
        public static string MakeTag(string input, PipelineCommand command, ExecutionLog log)
        {
            var namedArgs = new string[] {"tag", "content"};

            var tagName = command.GetArgument("tag");
            var contents = command.GetArgument("content", input);
            var attributes = command.CommandArgs.Where(ca => !namedArgs.Contains(ca.Key));

            var stringWriter = new StringWriter();
            var tagBuilder = new HtmlTextWriter(stringWriter);
           
            foreach (var attribute in attributes)
            {
                tagBuilder.AddAttribute(attribute.Key.ToString(), attribute.Value);
            }
            tagBuilder.RenderBeginTag(tagName);
            tagBuilder.WriteEncodedText(contents);
            tagBuilder.RenderEndTag();

            return stringWriter.ToString();

        }

        [Filter("SetAttribute", "Set a specific attribute on a tag.")]
        [Requires("AngleSharp.Parser.Html.HtmlParser, AngleSharp", "AngleSharp is an open-source markup parsing library.")]
        [ArgumentMeta("path", true, "The path to the tag.")]
        [ArgumentMeta("attribute", true, "The name of the attribute to set.")]
        [ArgumentMeta("val", true, "The new value of the attribute.")]
        [CodeSample(
            "<div data-number=\"006\">James Bond</div>",
            "Html.SetAttribute -path:div -attribute:data-number -value:007",
            "<div data-number=\"007\">James Bond</div>"
            )]
        public static string SetAttribute(string input, PipelineCommand command, ExecutionLog log)
        {
            var path = command.GetArgument("path");
            var attribute = command.GetArgument("attribute");
            var value = command.GetArgument("val");

            var parser = new HtmlParser(input);
            var doc = parser.Parse();
            var element = doc.QuerySelector(path);
            element.Attributes.ToList().First(a => a.Name == attribute).Value = value;

            return doc.DocumentElement.InnerHtml;
        }



    }
}