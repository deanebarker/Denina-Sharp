using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Xml;
using BlendInteractive.Denina.Core.Documentation;
using DeninaSharp.Core.Documentation;
using System.Configuration;
using DeninaSharp.Core.Filters;
using DeninaSharp.Core.Utility;

namespace DeninaSharp.Core.Filters
{
    [Filters("SQL", "For working with SQL databases.")]
    public class Sql
    {
        public static readonly string ALLOWED_CONNECTION_STRINGS_VARIABLE_NAME = "Sql.AllowedConnectionStrings";

        [Filter("GetXml", "Executes SQL against the specified connection string and returns an XML recordset.")]
        [ArgumentMeta(1, "Connection String Name", true, "The name of a connection string key from the application configuration.")]
        [ArgumentMeta(2, "SQL", false, "The SQL to execute. If omitted, the input string will be used.")]
        [CodeSample("SELECT * FROM TableName", "Sql.GetXml myConnectionString", "(An XML string)")]
        [CodeSample("", "Sql.GetXml myConnectionString \"SELECT * FROM TableName\"", "(An XML string)")]
        public static string GetXml(string input, PipelineCommand command)
        {
            var sql = input;
            var connectionStringName = command.GetArgument("connection");

            // If they passed in a SQL argument, use it
            if (command.HasArgument("sql"))
            {
                sql = command.GetArgument("sql");
            }

            IsValidConnectionStringName(connectionStringName);

            var sqlCommand = ConfigureCommand(connectionStringName, command);
            
            sqlCommand.CommandText = String.Concat(sql, " FOR XML PATH, ROOT('rows'), ELEMENTS XSINIL");

            var xml = "<rows/>";
            try
            {
                var reader = sqlCommand.ExecuteXmlReader();
                reader.Read();
                var rawResult = reader.ReadOuterXml();
                if (!String.IsNullOrWhiteSpace(rawResult))
                {
                    xml = rawResult;
                }

                var xmlDoc = new XmlDocument();

                try
                {
                    xmlDoc.LoadXml(xml);
                }
                catch (Exception e)
                {
                    throw new DeninaException("Unable to parse XML response.", e);
                }

            }
            catch (Exception e)
            {
                throw new DeninaException("Error executing SQL", e);
            }

            return String.IsNullOrWhiteSpace(xml) ? ">" : xml;
        }

        [Filter("GetTable", "Executes SQL against the specified connection string and returns an HTML table of the results.")]
        [ArgumentMeta(1, "Connection String Name", true, "The name of a connection string key from the application configuration.")]
        [ArgumentMeta(2, "SQL", false, "The SQL to execute. If omitted, the input string will be used.")]
        [CodeSample("SELECT * FROM TableName", "Sql.GetXml myConnectionString", "(An HTML table)")]
        [CodeSample("", "Sql.GetXml myConnectionString \"SELECT * FROM TableName\"", "(An HTML table)")]
        public static string GetTable(string input, PipelineCommand command)
        {
            var sql = input;
            var connectionStringName = command.GetArgument("connection");

            // If they passed in a SQL argument, use it
            if (command.HasArgument("sql"))
            {
                sql = command.GetArgument("sql");
            }

            IsValidConnectionStringName(connectionStringName);

            var sqlCommand = ConfigureCommand(connectionStringName, command);

            sqlCommand.CommandText = sql;

            SqlDataReader reader;
            try
            {
                reader = sqlCommand.ExecuteReader();
            }
            catch (Exception e)
            {
                throw new DeninaException("Error executing SQL", e);
            }

            var html = new StringWriter();
            var writer = new HtmlTextWriter(html);

            if (command.HasArgument("cssClass"))
            {
                writer.AddAttribute("class", command.GetArgument("cssClass"));
            }
            
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
            writer.RenderBeginTag(HtmlTextWriterTag.Thead);
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
            foreach (var column in columns)
            {
                var columnClass = column.ToLower().Replace(" ", "-");
                
                writer.AddAttribute("class", String.Concat("column-", columnClass));
                writer.RenderBeginTag(HtmlTextWriterTag.Th);
                writer.Write(column);
                writer.RenderEndTag();
            }

            writer.RenderEndTag(); // Ends tr
            writer.RenderEndTag(); // Ends thead

            writer.RenderBeginTag(HtmlTextWriterTag.Tbody);

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.IsDBNull(i) ? String.Empty : reader.GetString(i);
                        var columnClass = reader.GetName(i).ToLower().Replace(" ", "-");

                        writer.AddAttribute("class", String.Concat("column-", columnClass));
                        writer.RenderBeginTag(HtmlTextWriterTag.Td);
                        writer.Write(StringUtilities.Link(value));
                        writer.RenderEndTag(); // Ends td
                    }

                    writer.RenderEndTag(); // Ends tr
                }
            }

            writer.RenderEndTag(); // Ends tbody

            writer.RenderEndTag(); // End table

            return html.ToString();

        }

        private static SqlCommand ConfigureCommand(string connectionStringName, PipelineCommand command)
        {
            var sqlCommand = new SqlCommand()
            {
                Connection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionStringName].ToString())
            };

            // Add all variables as params
            foreach (var variable in command.Pipeline.Variables)
            {
                sqlCommand.Parameters.AddWithValue(variable.Key, variable.Value.Value ?? String.Empty);
            }

            try
            {
                sqlCommand.Connection.Open();
            }
            catch (Exception e)
            {

                throw new DeninaException("Error connecting to SQL Server with connection string \"" + connectionStringName + "\"", e);
            }
            

            return sqlCommand;
        }

        private static bool IsValidConnectionStringName(string connectionStringName)
        {
            // Have they set the allowed connection strings variable?
            if (!Pipeline.IsSetGlobally(ALLOWED_CONNECTION_STRINGS_VARIABLE_NAME))
            {
                throw new DeninaException(String.Format("Allowed connection strings must be set as the \"{0}\" variable.", ALLOWED_CONNECTION_STRINGS_VARIABLE_NAME));
            }

            // Is this connection string authorized?
            var allowedConnectionStrings = Pipeline.GetGlobalVariable(ALLOWED_CONNECTION_STRINGS_VARIABLE_NAME).ToString();
            if (!allowedConnectionStrings.Split(',').Contains(connectionStringName))
            {
                throw new DeninaException(String.Format("Connection string key \"{0}\" has not been authorized.", connectionStringName));
            }

            // Does this connection string exist?
            if (ConfigurationManager.ConnectionStrings[connectionStringName] == null)
            {
                throw new DeninaException(String.Format("Connection string \"{0}\" does not exist.", connectionStringName));
            }

            return true;
        }
    }
}
