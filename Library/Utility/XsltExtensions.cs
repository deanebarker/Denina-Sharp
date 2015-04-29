using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
namespace DeninaSharp.Core.Utility
{

    public class XsltExtensions
    {
        public string FormatDate(string input, string format, int hoursCorrection = 0)
        {
            return DateTime.Parse(input).AddHours(hoursCorrection).ToLocalTime().ToString(format);
        }

        public string Lower(string input)
        {
            return input.ToLower();
        }

        public string Replace(string input, string oldString, string newString)
        {
            return input.Replace(oldString, newString);
        }

        public string Link(string input)
        {
            return StringUtilities.Link(input);
        }

    }
}