using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeninaSharp.Core.Configuration
{
    public class PipelineConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("variables", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(PipelineConfigVariable))]
        public PipelineConfigVariables Variables
        {
            get { return (PipelineConfigVariables)base["variables"]; }
        }
    }
}
