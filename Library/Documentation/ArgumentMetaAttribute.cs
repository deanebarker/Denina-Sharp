using System;
using System.Xml.Serialization;

namespace DeninaSharp.Core.Documentation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [XmlRoot(ElementName = "argumentMeta")]
    public class ArgumentMetaAttribute : Attribute
    {
        public ArgumentMetaAttribute()
        {
            // For serailizing
        }

        public ArgumentMetaAttribute(string name, bool required, string description)
        {
            Name = name;
            Required = required;
            Description = description;
        }

        public string Name { get; set; }
        public bool Required { get; set; }
        public string Description { get; set; }
    }
}