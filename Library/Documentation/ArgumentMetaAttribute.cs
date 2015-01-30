using System;

namespace BlendInteractive.Denina.Core.Documentation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ArgumentMetaAttribute : Attribute
    {
        public ArgumentMetaAttribute(int order, string name, bool required, string description)
        {
            Name = name;
            Ordinal = order;
            Required = required;
            Description = description;
        }

        public int Ordinal { get; private set; }
        public string Name { get; private set; }
        public bool Required { get; private set; }
        public string Description { get; private set; }
    }
}