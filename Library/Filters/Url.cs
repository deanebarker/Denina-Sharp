using DeninaSharp.Core.Documentation;
using System;
using System.Collections.Specialized;
using System.Web;

namespace DeninaSharp.Core.Filters
{
    [Filters("Url", "Working with URLs.")]
    public static class Url
    {
        [Filter("AddQuerystringArg", "Adds a querystring argument (key/value pair) to a URL.")]
        [ArgumentMeta("key", true, "The key for the pair.")]
        [ArgumentMeta("value", true, "The value for the pair.")]
        [CodeSample("http://denina.org", "Url.AddQuerystringArg -key:a -value:b", "http://denina.org?a=b")]
        public static string AddQuerysgtringArg(string input, PipelineCommand command)
        {
            var uriBuilder = new UriBuilder(input);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[command.CommandArgs["1"]] = command.CommandArgs["2"];
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri.AbsoluteUri;
        }
    }
}