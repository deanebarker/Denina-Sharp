using System;
using System.Xml.Serialization;

namespace DeninaSharp.Core.Documentation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [XmlRoot(ElementName = "filterMeta")]
    public class FilterAttribute : Attribute
    {
        public FilterAttribute()
        {
            // This only exists for serializing
        }

        public FilterAttribute(string name, string description = null)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}