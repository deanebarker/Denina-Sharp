using System;
using Newtonsoft.Json.Linq;

namespace BlendInteractive.TextFilterPipeline.Core.Filters
{
    [TextFilters("JSON")]
    public static class Json
    {
        [TextFilter("ExtractFromJson")]
        public static string ExtractFromJson(string input, TextFilterCommand command)
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