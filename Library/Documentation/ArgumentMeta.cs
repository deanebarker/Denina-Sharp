using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlendInteractive.TextFilterPipeline.Core.Documentation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ArgumentMetaAttribute : Attribute
    {
        public int Ordinal { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }
        public string Description { get; set; }

        public ArgumentMetaAttribute(int order, string name, bool required, string description)
        {
            Name = name;
            Ordinal = order;
            Required = required;
            Description = description;
        }
    }

    
}
