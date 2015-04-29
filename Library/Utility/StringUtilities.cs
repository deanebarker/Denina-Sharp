using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;

namespace DeninaSharp.Core.Utility
{
    class StringUtilities
    {
        public static string Link(string input)
        {
            Uri uri;
            if (Uri.TryCreate(input, UriKind.Absolute, out uri))
            {
                // This is a URL...

                var html = new StringWriter();
                var writer = new HtmlTextWriter(html);

                writer.AddAttribute("href", input);
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(input);
                writer.RenderEndTag();

                return html.ToString();
            }

            // Quick check first...
            if (input.Contains("@"))
            {
                if (Regex.IsMatch(input, @"^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$", RegexOptions.IgnoreCase))
                {
                    // This is an email address...
                    
                    var html = new StringWriter();
                    var writer = new HtmlTextWriter(html);

                    writer.AddAttribute("href", String.Concat("mailto:", input));
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.Write(input);
                    writer.RenderEndTag();

                    return html.ToString();
                }
            }

            // It's neither, return it...
            return input;
        }
    }
}
