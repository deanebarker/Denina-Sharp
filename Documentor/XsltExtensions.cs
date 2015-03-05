using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Documentor
{
    public class XsltExtensions
    {
        public string Replace(string input, string find, string replace)
        {
            return input.Replace(find, replace);
        }

        public XPathNavigator NewLines(string input)
        {
            // From XSLT, the "\n" comes through literally...
            input = input.Replace("\n", Environment.NewLine);

            var newString = String.Join("<br/>", input.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));

           return GetFragment(newString);
        }

        public string CleanFileName(string input)
        {
            input = input.Replace(" ", "-");
            input = input.ToLower();
            input = Regex.Replace(input, @"\W", String.Empty);

            return input;
        }

        private XPathNavigator GetFragment(string input)
        {
            var settings = new XmlReaderSettings()
            {
                ConformanceLevel = ConformanceLevel.Fragment
            };

            var doc = new XPathDocument(XmlReader.Create(new StringReader(input), settings));
            return doc.CreateNavigator();
        }
    }

}
