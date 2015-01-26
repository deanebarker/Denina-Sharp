using System;
using System.Net;

namespace BlendInteractive.TextFilterPipeline.Core.Filters
{
    [TextFilters("HTTP")]
    public static class Http
    {
        [TextFilter("Get")]
        public static string Get(string input, TextFilterCommand command)
        {
            string url = command.CommandArgs.ContainsKey(0) ? command.CommandArgs[0] : input;

            if (!url.ToLower().StartsWith("http"))
            {
                url = String.Concat("http://", url);
            }

            var client = new WebClient();
            return client.DownloadString(url);
        }
    }
}