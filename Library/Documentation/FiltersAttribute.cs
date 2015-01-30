using System;

namespace BlendInteractive.Denina.Core.Documentation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FiltersAttribute : Attribute
    {
        public FiltersAttribute(string category, string description = null)
        {
            Category = category;
            if (!String.IsNullOrWhiteSpace(description))
            {
                Description = description;
            }
        }

        public string Category { get; private set; }
        public string Description { get; private set; }
    }
}