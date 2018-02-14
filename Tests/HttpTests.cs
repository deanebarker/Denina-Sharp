using System;
using DeninaSharp.Core;
using DeninaSharp.Core.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class HttpTests : BaseTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            // This will run after every test and clear the variable...
            Pipeline.ClearGlobalVariables();
        }

        [TestMethod]
        public void GetFromArgument()
        {
            Pipeline.SetGlobalVariable(Http.ALLOWED_DOMAINS_VARIABLE_NAME, "gadgetopia.com");
            var pipeline = new Pipeline();
            pipeline.AddCommand("HTTP.Get -url:http://gadgetopia.com/");
            var result = pipeline.Execute("");

            Assert.IsTrue(result.ToLower().Contains("gadgetopia"));
        }

        [TestMethod]
        public void GetFromInput()
        {
            Pipeline.SetGlobalVariable(Http.ALLOWED_DOMAINS_VARIABLE_NAME, "gadgetopia.com");
            var pipeline = new Pipeline();
            pipeline.AddCommand("HTTP.Get");
            var result = pipeline.Execute("http://gadgetopia.com/");

            Assert.IsTrue(result.ToLower().Contains("gadgetopia"));
        }

        [TestMethod]
        public void AuthorizedDomainsNotSet()
        {
            // Note that we're NOT setting the global variable here...
            var pipeline = new Pipeline();
            pipeline.AddCommand("HTTP.Get -url:http://gadgetopia.com/");

            try
            {
                var result = pipeline.Execute("");
            }
            catch (Exception e)
            {
                // It's good if it gets here
                Assert.AreEqual(e.Message, "Allowed domains variable must be defined.");
                return;
            }
            
            Assert.Fail("Shouldn't have gotten here...");
        }

        [TestMethod]
        public void AttemptUnauthorizedDomain()
        {
            Pipeline.SetGlobalVariable(Http.ALLOWED_DOMAINS_VARIABLE_NAME, "cnn.com");
            var pipeline = new Pipeline();
            pipeline.AddCommand("HTTP.Get -url:http://gadgetopia.com/");
            try
            {
                var result = pipeline.Execute("");
            }
            catch (Exception e)
            {
                // It's good if it gets here
                Assert.IsTrue(e.Message.StartsWith("Host not authorized"));
                return;
            }
            Assert.Fail("Shouldn't have gotten here...");
        }

        [TestMethod]
        public void AttemptInvalidUrl()
        {
            Pipeline.SetGlobalVariable(Http.ALLOWED_DOMAINS_VARIABLE_NAME, "gadgetopia.com");
            var pipeline = new Pipeline();
            pipeline.AddCommand("HTTP.Get -url:gadgetopia.com");
            try
            {
                var result = pipeline.Execute("");
            }
            catch (Exception e)
            {
                // It's good if it gets here
                Assert.AreEqual("Invalid URL provided.", e.Message);
                return;
            }
            Assert.Fail("Shouldn't have gotten here...");
        }
    }
}