using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlendInteractive.Denina.Core.Documentation;
using DeninaSharp.Core.Documentation;
using System.Configuration;
using DeninaSharp.Core.Filters;

namespace DeninaSharp.Core.Filters
{
    [Filters("SQL", "For working with SQL databases.")]
    public class Sql
    {
        public static readonly string ALLOWED_CONNECTION_STRINGS_VARIABLE_NAME = "__allowsqlconnections";

        [Filter("GetXml", "Executes SQL against the specified connection string and returns an XML recordset.")]
        [ArgumentMeta(1, "Connection String Name", true, "The name of a connection string key from the application configuration.")]
        [ArgumentMeta(2, "SQL", false, "The SQL to execute. If omitted, the input string will be used.")]
        [CodeSample("SELECT * FROM TableName", "Sql.GetXml myConnectionString", "(An XML string)")]
        [CodeSample("", "Sql.GetXml myConnectionString \"SELECT * FROM TableName\"", "(An XML string)")]
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

namespace DeninaSharp.Core
{
    public partial class Pipeline
    {
        public static void SetAllowedConnectionStrings(params string[] connectionStrings)
        {
            SetGlobalVariable(Sql.ALLOWED_CONNECTION_STRINGS_VARIABLE_NAME, String.Join(",", connectionStrings), true);
        }
    }
}
