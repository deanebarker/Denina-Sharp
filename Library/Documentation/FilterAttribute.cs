using System;

namespace BlendInteractive.Denina.Core.Documentation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class FilterAttribute : Attribute
    {
        public FilterAttribute(string name, string description = null)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
    }
}