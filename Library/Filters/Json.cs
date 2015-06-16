using System;
using System.Text.RegularExpressions;
using BlendInteractive.Denina.Core.Documentation;
using DeninaSharp.Core.Documentation;
using Newtonsoft.Json.Linq;

namespace DeninaSharp.Core.Filters
{
    [Filters("JSON", "Working with JSON data.")]
    public static class Json
    {
        [Filter("Extract", "Retieves the valud at a defined \"path\" into the JSON object.")]
        [ArgumentMeta("path", true, "A dot-delimited representation of the path into the object. Each property is defined by a segment. Numeric values prepended by a tilde (\"~1\") are interpreted as the index of an array. Example: \"person.~2.name.first\".")]
        [CodeSample("(A JSON String)", "Json.Extract -path:person.~1.name", "(The value of the \"name\" object of the second \"person\" object.)")]       
        public static string ExtractFromJson(string input, PipelineCommand command)
        {
            var path = command.GetArgument("path");

            var jobject = (JToken) JObject.Parse(input);

            foreach (string segment in path.Split('.'))
            {
                if (Regex.IsMatch(segment,@"\[[0-9]+\]"))
                {
                    jobject = jobject[Convert.ToInt32(segment.Replace("[", String.Empty).Replace("]", String.Empty))];
                    continue;
                }

                if (jobject[segment] is JValue)
                {
                    return jobject[segment].ToString();
                }

                jobject = jobject[segment];
            }

            return String.Empty;
        }
    }
}