using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BlendInteractive.TextFilterPipeline.Core
{
    public class TextFilterCommand
    {
        public TextFilterPipeline Pipeline;
        public string CommandName { get; set; }
        public Dictionary<object, string> CommandArgs { get; set; }
        public string VariableName { get; set; }

        public TextFilterCommand()
        {
            CommandArgs = new Dictionary<object, string>();
        }       

        public string DefaultArgument
        {
            get { return CommandArgs.FirstOrDefault().Value; }
        }

        public string NormalizedCommandName
        {
            get { return CommandName.ToLower(); }
        }
    }
}