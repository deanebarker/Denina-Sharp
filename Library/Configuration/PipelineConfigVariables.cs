using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeninaSharp.Core.Configuration
{
    
    public class PipelineConfigVariables : ConfigurationElementCollection
    {
        private Dictionary<string, string> variables = new Dictionary<string, string>();

        public PipelineConfigVariable this[int index]
        {
            get
            {
                return BaseGet(index) as PipelineConfigVariable;
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new PipelineConfigVariable();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PipelineConfigVariable) element).Key;
        }
    }

}
