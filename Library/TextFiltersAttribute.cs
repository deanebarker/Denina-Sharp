using System;
using System.Security.Cryptography;

namespace BlendInteractive.TextFilterPipeline.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TextFiltersAttribute : Attribute
    {
        public TextFiltersAttribute(string category, string description = null)
        {
            Category = category;
            if(!String.IsNullOrWhiteSpace(description))
            {
                Description = description;
            }
        }

        public string Category { get; set; }
        public string Description { get; set; }
    }
}