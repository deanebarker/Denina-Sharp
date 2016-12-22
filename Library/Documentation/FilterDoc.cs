using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace DeninaSharp.Core.Documentation
{



    public class FilterDoc
    {
        public FilterDoc(MethodInfo method)
        {
            Arguments = new List<ArgumentDoc>();
            Samples = new List<CodeSampleDoc>();
            Dependencies = new List<DependencyDoc>();

            var filterAttribute = method.GetCustomAttribute<FilterAttribute>();
            if (filterAttribute == null)
            {
                Name = method.Name;
            }
            else
            {
                Name = filterAttribute.Name;
                Description = filterAttribute.Description;
            }

            if (method.GetCustomAttributes(typeof(ArgumentMetaAttribute), true).Any())
            {

                var argumentAttribute = method.GetCustomAttributes<ArgumentMetaAttribute>();
                foreach (var attribute in argumentAttribute)
                {
                    var argument = new ArgumentDoc()
                    {
                        Description = attribute.Description,
                        Name = attribute.Name,
                        Required = attribute.Required
                    };
                    Arguments.Add(argument);
                }
            }

            if (method.GetCustomAttributes(typeof(RequiresAttribute), true).Any())
            {
                var requiresAttribute = method.GetCustomAttributes<RequiresAttribute>();
                foreach (var attribute in requiresAttribute)
                {
                    var dependency = new DependencyDoc()
                    {
                        TypeName = attribute.TypeName,
                        Note = attribute.Note
                    };
                    Dependencies.Add(dependency);
                }
            }

            if (method.GetCustomAttributes(typeof(CodeSampleAttribute), true).Any())
            {

                var samplesAttribute = method.GetCustomAttributes<CodeSampleAttribute>();
                foreach (var attribute in samplesAttribute)
                {
                    var sample = new CodeSampleDoc()
                    {
                        Input = attribute.Input,
                        Command = attribute.Command,
                        Output = attribute.Output
                    };
                    Samples.Add(sample);
                }
            }
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<ArgumentDoc> Arguments;
        public List<CodeSampleDoc> Samples;
        public List<DependencyDoc> Dependencies;
    }

}