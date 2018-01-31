using DeninaSharp.Core;
using DeninaSharp.Core.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class SqlTests : BaseTests
    {
        [TestMethod]
        public void DisallowedConnectionString()
        {
            var pipeline = new Pipeline();
            // "a" is not a valid connection string
            pipeline.AddCommand("Sql.GetXml -connection:a");
            try
            {
                pipeline.Execute();
            }
            catch(DeninaException e)
            {
                Assert.IsTrue(e.Message.Contains("authorized"));
            }
        }

        [TestMethod]
        public void AllowedConnectionString()
        {
            var pipeline = new Pipeline();
            // "a" is not a valid connection string
            pipeline.AddCommand("Sql.GetXml -connection:x");
            try
            {
                pipeline.Execute();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.InnerException != null);
                Assert.IsTrue(e.InnerException.Message.Contains("Format of the initialization string"));
            }
        }
    }
}
