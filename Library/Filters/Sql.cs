using DeninaSharp.Core.Documentation;
using DeninaSharp.Core.Utility;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Xml;

namespace DeninaSharp.Core.Filters
{
    [Filters("SQL", "For working with SQL databases.")]
    public class Sql
    {
        public static readonly string ALLOWED_CONNECTION_STRINGS_VARIABLE_NAME = "Sql.AllowedConnectionStrings";

        [Filter("GetXml", "Executes SQL against the specified connection string and returns an XML recordset.")]
        [ArgumentMeta("connection", true, "The name of a connection string key from the application configuration.")]
        [ArgumentMeta("sql", false, "The SQL to execute. If omitted, the input string will be used.")]
        [CodeSample("SELECT * FROM TableName", "Sql.GetXml -connection:myConnectionString", "(An XML string)")]
        [CodeSample("(None)", "Sql.GetXml -connection:myConnectionString -sql:\"SELECT * FROM TableName\"", "(An XML string)")]
        public static string GetXml(string input, PipelineCommand command)
        {
            var sql = command.GetArgument("sql,proc", input);
            var connectionStringName = command.GetArgument("connection");

            IsValidConnectionStringName(connectionStringName);

            var sqlCommand = ConfigureCommand(connectionStringName, command);

            // If this isn't a stored proc, then we need to append the XML stuff to it.  If it is a stored proc, when we assume that's already there.
            if (!command.HasArgument("proc"))
            {
                sqlCommand.CommandText = String.Concat(sql, " FOR XML PATH, ROOT('rows'), ELEMENTS XSINIL");
            }
            else
            {
                sqlCommand.CommandText = sql;
            }

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

        [Filter("GetTable", "Executes SQL against the specified connection string and returns an HTML table of the results. TH tags contain the field names of the returned dataset and each TD and TH has a CSS class of \"column-field-name\" for styling.")]
        [ArgumentMeta("connecton", true, "The name of a connection string key from the application configuration. This connection string name must be enabled via a Sql.AllowedConnectionStrings setting.")]
        [ArgumentMeta("sql", false, "The SQL to execute. If omitted, the input string will be used.")]
        [ArgumentMeta("class", false, "A CSS class name for the TABLE tag.")]
        [CodeSample("SELECT * FROM TableName", "Sql.GetXml -connection:myConnectionString", "(An HTML table)")]
        [CodeSample("(None)", "Sql.GetXml -connection:myConnectionString -sql:\"SELECT * FROM TableName\" -class:table", "(An HTML table with a \"class\" attribute of \"table\")")]
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

            if (command.HasArgument("class"))
            {
                writer.AddAttribute("class", command.GetArgument("class"));
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
                        var value = reader.IsDBNull(i) ? String.Empty : Convert.ToString(reader[i]);
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

        private static SqlCommand ConfigureCommand(string connectionInfo, PipelineCommand command)
        {
            // Determine if we have an actual connection string, or a connection string name
            var connectionString = IsConnectionStringName("Data Source=") ? ConfigurationManager.ConnectionStrings[connectionInfo].ToString() : connectionInfo;

            var sqlCommand = new SqlCommand()
            {
                Connection = new SqlConnection(connectionString)
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

                throw new DeninaException("Error connecting to SQL Server with connection string \"" + connectionInfo + "\"", e);
            }
            

            return sqlCommand;
        }

        private static bool IsValidConnectionStringName(string connectionStringName)
        {
            // If it's not a connection string name, then we're not going to authorize it
            // We're only worried about connection string names because that's shorthand for a connection string you might not know otherwise
            // If you have the actual connection string, we don't care. If you have it you have it.
            // Our concern is that a connection string name gives you access to connection credentials you don't know in advance.
            if (!IsConnectionStringName(connectionStringName))
            {
                return true;
            }

            // Have they set the allowed connection strings variable?
            if (!Pipeline.IsSetGlobally(ALLOWED_CONNECTION_STRINGS_VARIABLE_NAME))
            {
                throw new DeninaException(String.Format("Allowed connection strings must be set as the \"{0}\" variable.", ALLOWED_CONNECTION_STRINGS_VARIABLE_NAME));
            }

            // Is this connection string authorized?
            var allowedConnectionStrings = Pipeline.GetGlobalVariable(ALLOWED_CONNECTION_STRINGS_VARIABLE_NAME).ToString();
            if (!allowedConnectionStrings.Split(",".ToCharArray()).Select(s => s.Trim().ToLower()).Contains(connectionStringName.ToLower()))
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

        private static bool IsConnectionStringName(string input)
        {
            return (ConfigurationManager.ConnectionStrings[input] != null);
        }
    }
}
