using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeninaSharp.Core.Documentation;

namespace DeninaSharp.Core.Filters
{
    [Filters("If", "For handling conditional execution.")]
    public class If
    {
        [Filter("NoVar", "Goes to the specified label if the specified variable has not been initialized.")]
        [ArgumentMeta(1, "Variable Name", true, "The variable to check for a value.")]
        [ArgumentMeta(2, "Label", true, "The label to find if the specified variable has no value.")]
        public static string NoVar(string input, PipelineCommand command)
        {
            var varName = command.CommandArgs.First().Value;
            var labelName = command.CommandArgs[1];

            if (!command.Pipeline.IsSet(varName))
            {
                command.SendToLabel = labelName;
            }

            return input;
        }

        [Filter("IsEmpty", "Goes to the specified label if the input text is empty.")]
        [ArgumentMeta(1, "Label", true, "The label to find if the input text is empty.")]
        public static string IsEmpty(string input, PipelineCommand command)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                command.SendToLabel = command.DefaultArgument;
            }

            return input;
        }

        [Filter("IsEqualTo", "Goes to the specified label if the input text is equal.")]
        [ArgumentMeta(1, "Label", true, "The label to find if the input text is empty.")]
        [ArgumentMeta(1, "Value", true, "The value to check against.")]
        public static string IsEqualTo(string input, PipelineCommand command)
        {
            var value = command.GetArgument("value");

            if(input == value)
            {
                command.SendToLabel = command.DefaultArgument;
            }

            return input;
        }

    }
}
