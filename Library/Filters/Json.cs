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
        [ArgumentMeta("path", true, "A dot-delimited representation of the path into the object. Each property is defined by a segment. Example: \"person[2].name.first\".")]
        [CodeSample("(A JSON String)", "Json.Extract -path:person[1].name", "(The value of the \"name\" object of the second \"person\" object.)")]       
        public static string ExtractFromJson(string input, PipelineCommand command)
        {
            var path = command.GetArgument(key: "path");

            var jobject = (JToken)JObject.Parse(json: input);

            return (string)jobject.SelectToken(path: path);
        }
    }
}