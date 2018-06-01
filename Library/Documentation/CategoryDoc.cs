using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DeninaSharp.Core.Documentation;

namespace DeninaSharp.Core.Documentation
{
    public class CategoryDoc : BaseDoc
    {
        public CategoryDoc(Type type)
        {
            var attribute = ((FiltersAttribute)type.GetCustomAttributes(typeof(FiltersAttribute), true).FirstOrDefault());
            Name = attribute?.Category ?? type.Name;
            Description = Eval(attribute?.Description);
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}