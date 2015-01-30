using System;
using BlendInteractive.Denina.Core.Documentation;
using Newtonsoft.Json.Linq;

namespace BlendInteractive.Denina.Core.Filters
{
    [Filters("JSON", "Working with JSON data.")]
    public static class Json
    {
        [Filter("ExtractFromJson", "Retieves the valud at a defined \"path\" into the JSON object.")]
        [ArgumentMeta(1, "Path", true, "A dot-delimited representation of the path into the object. Each property is defined by a segment. Numeric values prepended by a tilde (\"~1\") are interpreted as the index of an array. Example: \"person.~2.name.first\".")]
        public static string ExtractFromJson(string input, PipelineCommand command)
        {
            var jobject = (JToken) JObject.Parse(input);

            foreach (string segment in command.CommandArgs["1"].Split('.'))
            {
                if (segment[0] == '~')
                {
                    jobject = jobject[Convert.ToInt32(segment.Replace("~", String.Empty))];
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