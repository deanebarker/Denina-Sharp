using System;
using System.Security.Cryptography;

namespace BlendInteractive.TextFilterPipeline.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TextFilterAttribute : Attribute
    {
        public TextFilterAttribute(string name, string description = null)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}