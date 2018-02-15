using DeninaSharp.Core.Documentation;
using Newtonsoft.Json.Linq;

namespace DeninaSharp.Core.Filters
{
    [Filters("JSON", "Working with JSON data.")]
    public static class Json
    {
        [Filter("Extract", "Retieves the valud at a defined \"path\" into the JSON object.")]
        [ArgumentMeta("path", true, "A valid JSON path, as accepted by the Newtonsoft JObject.SelectToken method (see: http://www.newtonsoft.com/json/help/html/SelectToken.htm).")]
        [CodeSample("resource:json-extract-input.json", "Json.Extract -path:person.name", "James Bond")]     
        [Requires("Newtonsoft.Json.Linq.JObject, Newtonsoft.Json", "This is in the JSON.NET Nuget package.")]  
        [Requires("Newtonsoft.Json.Linq.JToken, Newtonsoft.Json", "This is in the JSON.NET Nuget package.")]  
        public static string ExtractFromJson(string input, PipelineCommand command, ExecutionLog log)
        {
            var path = command.GetArgument(key: "path");

            var jobject = (JToken)JObject.Parse(json: input);

            return (string)jobject.SelectToken(path: path);
        }
    }
}