using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeninaSharp.Core.Configuration
{
    public class PipelineConfigVariable : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true, IsKey = true)]
        public string Key
        {
            get { return (string)this["key"]; }
        }

        [ConfigurationProperty("value", IsRequired = true, IsKey = false)]
        public string Value
        {
            get { return (string)this["value"]; }
        }
    }
}
