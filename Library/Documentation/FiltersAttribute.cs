using System;
using System.Xml.Serialization;

namespace DeninaSharp.Core.Documentation
{
    [AttributeUsage(AttributeTargets.Class)]
    [XmlRoot(ElementName = "categoryMeta")]
    public class FiltersAttribute : Attribute
    {
        public FiltersAttribute()
        {
            // This only exists for serializing
        }

        public FiltersAttribute(string category, string description = null)
        {
            Category = category;
            if (!String.IsNullOrWhiteSpace(description))
            {
                Description = description;
            }
        }

        public string Category { get; set; }
        public string Description { get; set; }
    }
}