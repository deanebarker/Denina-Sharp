using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BlendInteractive.Denina.Core.Documentation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [XmlRoot(ElementName = "dependency")]
    public class RequiresAttribute : Attribute
    {
        public string TypeName { get; set; }
        public string Note { get; set; }

        public RequiresAttribute()
        {
            
        }

        public RequiresAttribute(string typeName, string note)
        {
            TypeName = typeName;
            Note = note;
        }
    }
}
