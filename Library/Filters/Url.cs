using System;
using System.Collections.Specialized;
using System.Web;
using BlendInteractive.TextFilterPipeline.Core.Documentation;

namespace BlendInteractive.TextFilterPipeline.Core.Filters
{
    [TextFilters("Url", "Working with URLs.")]
    public static class Url
    {
        [TextFilter("AddQuerystringArg", "Adds a querystring argument (key/value pair) to a URL.")]
        [ArgumentMeta(1, "Key", true, "The key for the pair.")]
        [ArgumentMeta(1, "Value", true, "The value for the pair.")]
        public static string AddQuerysgtringArg(string input, TextFilterCommand command)
        {
            var uriBuilder = new UriBuilder(input);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[command.CommandArgs["1"]] = command.CommandArgs["2"];
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri.AbsoluteUri;
        }
    }
}