using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlendInteractive.Denina.Core.Documentation;
using System.Configuration;
using BlendInteractive.Denina.Core.Filters;

namespace BlendInteractive.Denina.Core.Filters
{
    [Filters("SQL", "For working with SQL databases.")]
    public class Sql
    {
        public static readonly string ALLOWED_CONNECTION_STRINGS_VARIABLE_NAME = "__allowsqlconnections";

        [Filter("GetXml")]
        public static string GetXml(string input, PipelineCommand command)
        {
            var allowedConnectionStrings = Pipeline.GetGlobalVariable(ALLOWED_CONNECTION_STRINGS_VARIABLE_NAME).ToString();
            if (!allowedConnectionStrings.Split(',').Contains(command.CommandArgs.First().Value))
            {
                throw new Exception(String.Format("Connection string key \"{0}\" has not been authorized.", command.CommandArgs.First().Value));
            }

            var sqlCommand = new SqlCommand()
            {
                Connection = new SqlConnection(ConfigurationManager.ConnectionStrings[command.CommandArgs.First().Value].ToString())
            };
            sqlCommand.Connection.Open();
            sqlCommand.CommandText = String.Concat(command.CommandArgs[1], " FOR XML PATH, ROOT('rows'), ELEMENTS XSINIL");

            var reader = sqlCommand.ExecuteXmlReader();
            reader.Read();
            return reader.ReadOuterXml();
        }
    }
}

namespace BlendInteractive.Denina.Core
{
    public partial class Pipeline
    {
        public static void SetAllowedConnectionStrings(params string[] connectionStrings)
        {
            SetGlobalVariable(Sql.ALLOWED_CONNECTION_STRINGS_VARIABLE_NAME, String.Join(",", connectionStrings), true);
        }
    }
}
