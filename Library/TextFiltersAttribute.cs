using System;

namespace BlendInteractive.TextFilterPipeline.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TextFiltersAttribute : Attribute
    {
        public TextFiltersAttribute(string category)
        {
            Category = category;
        }

        public string Category { get; set; }
    }
}