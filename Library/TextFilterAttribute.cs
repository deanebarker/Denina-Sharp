using System;

namespace BlendInteractive.TextFilterPipeline.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TextFilterAttribute : Attribute
    {
        public TextFilterAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}