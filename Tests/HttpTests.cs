using BlendInteractive.TextFilterPipeline.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class HttpTests
    {
        [TestMethod]
        public void GetFromArgument()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("HTTP.Get http://gadgetopia.com/");
            string result = pipeline.Execute("");

            Assert.IsTrue(result.ToLower().Contains("gadgetopia"));
        }

        [TestMethod]
        public void GetFromInput()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("HTTP.Get");
            string result = pipeline.Execute("http://gadgetopia.com/");

            Assert.IsTrue(result.ToLower().Contains("gadgetopia"));
        }
    }
}