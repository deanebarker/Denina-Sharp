using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace DeninaSharp.Core.Documentation
{



    public class FilterDoc
    {
        // This constructor exists so that we can add a filter manually, without a FilterAttribute
        // Most filters will be reflected, but occasionally they'll want to add one manually, in which case we just take the string and description manually
        public FilterDoc(MethodInfo method, string name, string description)
        {
            Init(method, name, description);
        }

        // This constructor is no longer being used, since the other one was added
        // But I'm leaving this, with this comment, because sometime in the future, someone is gonna look at the other constructor and say,
        //    "Why don't we just reflect FilterAttribute for the name and description?"
        // Hopefully, they'll find this constructor, realize we tried this, and we had to add the other one because sometimes there *isn't* a FilterAttribute
        public FilterDoc(MethodInfo method, FilterAttribute filterAttribute)
        {
            Init(method, filterAttribute.Name, filterAttribute.Description);
        }

        public void Init(MethodInfo method, string name, string description)
        {
            Arguments = new List<ArgumentDoc>();
            Samples = new List<CodeSampleDoc>();
            Dependencies = new List<DependencyDoc>();

            Name = name;
            Description = description;

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