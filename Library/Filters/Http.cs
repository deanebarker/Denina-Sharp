using System;
using System.Net;
using BlendInteractive.Denina.Core.Documentation;
using DeninaSharp.Core.Documentation;

namespace DeninaSharp.Core.Filters
{
    [Filters("HTTP", "Working with resources over HTTP.")]
    public static class Http
    {
        [Filter("Get", "Makes an HTTP GET request and returns the result.")]
        [ArgumentMeta(1, "URL", false, "The URL to request. If not provided, the input string is assumed to be a URL.")]
        [CodeSample("", "Http.Get http://denina.org", "(The contents of the page at denina.org)")]
        [CodeSample("http://denina.org", "Http.Get", "(The contents of the page at denina.org)")]
        public static string Get(string input, PipelineCommand command)
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