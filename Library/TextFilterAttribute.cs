using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlendInteractive.TextFilterPipeline.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TextFilterAttribute : Attribute
    {
        public string Name { get; set; }

        public TextFilterAttribute(string name)
        {
            Name = name;
        }
    }
}