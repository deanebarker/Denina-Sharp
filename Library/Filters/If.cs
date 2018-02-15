using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DeninaSharp.Core.Documentation;

namespace DeninaSharp.Core.Filters
{
    [Filters("If", "For handling conditional execution.")]
    public class If
    {
        [Filter("NoVar", "Goes to the specified label if the specified variable has not been initialized.")]
        [ArgumentMeta("var", true, "The variable to check for a value.")]
        [ArgumentMeta("label", true, "The label to find if the specified variable has no value.")]
        public static string NoVar(string input, PipelineCommand command, ExecutionLog log)
        {
            var varName = command.GetArgument("var");
            var labelName = command.GetArgument("label");

            if (!command.Pipeline.IsSet(varName))
            {
                command.SendToLabel = labelName;
            }

            return input;
        }

        [Filter("IsEmpty", "Goes to the specified label if the input text is empty.")]
        [ArgumentMeta("label", true, "The label to find if the input text is empty.")]
        public static string IsEmpty(string input, PipelineCommand command, ExecutionLog log)
        {
            var value = command.GetArgument("value", input);
            var label = command.GetArgument("label");

            if (String.IsNullOrWhiteSpace(value))
            {
                command.SendToLabel = label;
            }

            return input;
        }

        [Filter("IsEqualTo", "Goes to the specified label if the input text is equal.")]
        [ArgumentMeta("label", true, "The label to find if the input text is empty.")]
        [ArgumentMeta("value", true, "The value to check against.")]
        public static string IsEqualTo(string input, PipelineCommand command, ExecutionLog log)
        {
            var value = command.GetArgument("value");

            if(input == value)
            {
                command.SendToLabel = command.DefaultArgument;
            }

            return input;
        }

        [Filter("IsInGroup", "Goes to the specified label if current user is in the specified group.")]
        [ArgumentMeta("group", true, "The group to check.")]
        [ArgumentMeta("label", true, "The label to go to if the check is true.")]
        public static string IsInGroup(string input, PipelineCommand command, ExecutionLog log)
        {
            if (HttpContext.Current == null)
            {
                return input;
            }

            var groups = command.GetMultiArgument("group");

            foreach (var group in groups)
            {
                if (HttpContext.Current.User.IsInRole(group))
                {
                    command.SendToLabel = command.GetArgument("label");
                    return input;
                }

            }

            return input;
        }

        [Filter("IsNotInGroup", "Goes to the specified label if current user is in the specified group.")]
        [ArgumentMeta("group", true, "The group to check.")]
        [ArgumentMeta("label", true, "The label to go to if the check is true.")]
        public static string IsNotInGroup(string input, PipelineCommand command, ExecutionLog log)
        {
            if (HttpContext.Current == null)
            {
                return input;
            }

            var group = command.GetArgument("group");

            if (!HttpContext.Current.User.IsInRole(group))
            {
                command.SendToLabel = command.GetArgument("label");
                return input;
            }

            return input;
        }


    }
}
