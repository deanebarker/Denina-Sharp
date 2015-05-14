using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using BlendInteractive.Denina.Core.Documentation;
using DeninaSharp.Core.Documentation;

namespace DeninaSharp.Core.Filters
{
    [Filters("HTTP", "Working with resources over HTTP.")]
    public static class Http
    {
        public static readonly string ALLOWED_DOMAINS_VARIABLE_NAME = "Http.AllowedDomains";
        public static readonly string ALL_DOMAINS_WILDCARD = "*";


        [Filter("Get", "Makes an HTTP GET request and returns the result.")]
        [ArgumentMeta("url", false, "The URL to request. If not provided, the input string is assumed to be a URL.")]
        [CodeSample("", "Http.Get http://denina.org", "(The contents of the page at denina.org)")]
        [CodeSample("http://denina.org", "Http.Get", "(The contents of the page at denina.org)")]
        public static string Get(string input, PipelineCommand command)
        {
            // The sandbox variable must be set...
            if (!Pipeline.IsSetGlobally(ALLOWED_DOMAINS_VARIABLE_NAME))
            {
                throw new DeninaException("Allowed domains variable must be defined.");
            }

            var url = command.GetArgument("url", input);

            Uri parsedUri;
            try
            {
                parsedUri = new Uri(url);
            }
            catch (Exception e)
            {
                throw new DeninaException("Invalid URL provided.", e);
            }

            // If it's set to the wildcard, we can skip the check entirely
            if (Pipeline.GetGlobalVariable(ALLOWED_DOMAINS_VARIABLE_NAME).ToString() != ALL_DOMAINS_WILDCARD)
            {

                var allowedDomains = Pipeline.GetGlobalVariable(ALLOWED_DOMAINS_VARIABLE_NAME).ToString().Split(',').Select(s => s.Trim().ToLower()).ToList();

                if (!allowedDomains.Contains(parsedUri.Host.ToLower()))
                {
                    throw new DeninaException(String.Concat("Host not authorized: ", parsedUri.Host));
                }
            }

            var client = new WebClient();
            return client.DownloadString(url);
        }
    }
}