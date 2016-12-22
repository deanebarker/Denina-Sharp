using DeninaSharp.Core.Documentation;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace DeninaSharp.Core.Filters
{
    [Filters("HTTP", "Working with resources over HTTP.")]
    public static class Http
    {
        public static readonly string ALLOWED_DOMAINS_VARIABLE_NAME = "Http.AllowedDomains";
        public static readonly string ALL_DOMAINS_WILDCARD = "*";


        [Filter("Get", "Makes an HTTP GET request and returns the result.")]
        [ArgumentMeta("url", false, "The URL to request. If not provided, the input string is assumed to be a URL.")]
        [CodeSample("", "Http.Get -url:http://denina.org", "(The contents of the page at denina.org)")]
        [CodeSample("http://denina.org", "Http.Get", "(The contents of the page at denina.org)")]
        public static string Get(string input, PipelineCommand command)
        {
            var url = GetUrlArgument(input, command);
            
            var client = new WebClient();
            return client.DownloadString(url);
        }

        [Filter("Proxy", "Proxies the current request to a specified URL.")]
        [ArgumentMeta("method", false, "GET or POST. If not provided, the same method is used as the current request.")]
        [ArgumentMeta("url", false, "The URL to request. If not provided, the input string is assumed to be a URL.")]
        [ArgumentMeta("get", false, "GET variables (querystring arguments) to pass-through to the proxy.")]
        [ArgumentMeta("post", false, "POST variables (form arguments) to pass-through to the proxy.")]
        [ArgumentMeta("header", false, "HTTP headers to pass-through to the proxy.")]
        [ArgumentMeta("cookie", false, "Cookies to pass-through to the proxy.")]
        [CodeSample("(Anything)", "Http.Proxy -url:http://gadgetopia.com/search -get:q -get:p", "(The results of the page at http://gadgetopia.com/search?q=[whatever]&p=[whatever] as if it were called with the same querystring arguments of \"q\" and \"p\" as the request to the current page.)")]
        public static string Proxy(string input, PipelineCommand command)
        {
            var method = command.GetArgument("method", HttpContext.Current.Request.HttpMethod);
            var get = command.GetMultiArgument("get");
            var post = command.GetMultiArgument("post");
            var cookie = command.GetMultiArgument("cookie");
            var header = command.GetMultiArgument("header");
            var url = GetUrlArgument(input, command);

            var builder = new UriBuilder(url);

            // Add querystring args
            var query = HttpUtility.ParseQueryString(builder.Query);
            foreach (var getItem in get)
            {
                if (HttpContext.Current.Request.QueryString.AllKeys.Contains(getItem))
                {
                    query[getItem] = HttpContext.Current.Request.QueryString[getItem];
                }
            }
            builder.Query = query.ToString();

            //var request = CreateProxyRequest(builder.Uri);
            var request = (HttpWebRequest)WebRequest.Create(builder.Uri);
            request.Method = method.ToUpper();

            // Add form args
            if (post.Count > 0)
            {
                var postBody = HttpUtility.ParseQueryString("");
                foreach (var postItem in post)
                {
                    postBody[postItem] = HttpContext.Current.Request.Form[postItem];
                }
                if (postBody.Count > 0)
                {
                    var buf = Encoding.UTF8.GetBytes(postBody.ToString());
                    request.ContentLength = buf.Length;
                    request.GetRequestStream().Write(buf, 0, buf.Length);
                }
            }

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
                throw new DeninaException("Proxy request returned an error.", e);
            }

            var responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                return String.Empty;
            }

            var enc = Encoding.UTF8;  // TODO: This is very poor form. I need to dynamically detect encoding...
            return new StreamReader(responseStream, enc).ReadToEnd();
        }

        private static string GetUrlArgument(string input, PipelineCommand command, string argumentName = "url")
        {
            // The sandbox variable must be set...
            if (!Pipeline.IsSetGlobally(ALLOWED_DOMAINS_VARIABLE_NAME))
            {
                throw new DeninaException("Allowed domains variable must be defined.");
            }

            var url = command.GetArgument(argumentName, input);


            // If this URL is relative, swap the URL into the current request, to assume the current host and scheme
            if (url.StartsWith("/"))
            {
                var currentUrl = HttpContext.Current.Request.Url;
                var builder = new UriBuilder();
                builder.Host = currentUrl.Host;
                builder.Scheme = currentUrl.Scheme;
                builder.Port = currentUrl.Port;
                builder.Path = url;
                url = builder.Uri.AbsoluteUri;
            }

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

            return url;
        }

        private static HttpWebRequest CreateProxyRequest(Uri uri, NameValueCollection additionalHeaders = null, HttpRequest source = null)
        {
            if (source == null)
            {
                if (HttpContext.Current == null)
                {
                    // The have to have one or the other.
                    throw new ArgumentException("HttpRequestBase was not passed in, and HttpContext.Current is null.");
                }
                source = HttpContext.Current.Request;
            }

            var destination = (HttpWebRequest)WebRequest.Create(uri.AbsoluteUri);

            destination.Method = source.HttpMethod;
            destination.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            // Copy unrestricted headers (including cookies, if any)
            //foreach (var headerKey in source.Headers.AllKeys)
            //{
            //    switch (headerKey)
            //    {
            //        case "Connection":
            //        case "Content-Length":
            //        case "Date":
            //        case "Expect":
            //        case "Host":
            //        case "If-Modified-Since":
            //        case "Range":
            //        case "Transfer-Encoding":
            //        case "Proxy-Connection":
            //            // Let IIS handle these
            //            break;

            //        case "Accept":
            //        case "Content-Type":
            //        case "Referer":
            //        case "User-Agent":
            //            // Restricted - copied below
            //            break;

            //        default:
            //            destination.Headers[headerKey] = source.Headers[headerKey];
            //            break;
            //    }
            //}

            // Copy any extra headers they passed in
            if (additionalHeaders != null && additionalHeaders.Count > 0)
            {
                foreach (var headerKey in additionalHeaders.AllKeys)
                {
                    destination.Headers[headerKey] = additionalHeaders[headerKey];
                }
            }


            // Copy restricted headers
            if (source.AcceptTypes.Any())
            {
                destination.Accept = string.Join(",", source.AcceptTypes);
            }
            destination.ContentType = source.ContentType;
            destination.Referer = source.UrlReferrer == null ? String.Empty : source.UrlReferrer.AbsoluteUri;
            destination.UserAgent = source.UserAgent;

            // Copy content (if content body is allowed)
            //if (source.HttpMethod != "GET"
            //    && source.HttpMethod != "HEAD"
            //    && source.ContentLength > 0)
            //{
            //    var destinationStream = destination.GetRequestStream();
            //    source.InputStream.CopyTo(destinationStream);
            //    destinationStream.Close();
            //}

            return destination;
        }
    }
}