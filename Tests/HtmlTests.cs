using BlendInteractive.TextFilterPipeline.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class HtmlTests
    {
        [TestMethod]
        public void Wrap()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("html.Wrap p theClass theId");
            string result = pipeline.Execute("Deane");

            Assert.AreEqual("<p class=\"theClass\" id=\"theId\">Deane</p>", result);
        }
    }
}