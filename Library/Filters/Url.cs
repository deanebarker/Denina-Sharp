using System;
using System.Collections.Specialized;
using System.Web;

namespace BlendInteractive.TextFilterPipeline.Core.Filters
{
    [TextFilters("Url")]
    public static class Url
    {
        [TextFilter("AddQuerystringArg")]
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