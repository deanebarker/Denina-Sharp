using BlendInteractive.TextFilterPipeline.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class HtmlTests
    {
        [TestMethod]
        public void InvalidXpathFailsBackToCssSelector()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("html.Extract h3.title");
            var result =
                pipeline.Execute(
                    "<html><head></head><body><h3 class='title'>2 + 2 = 10 ....in base 4</h3></body></html>");
            Assert.AreEqual("2 + 2 = 10 ....in base 4",result);

        }

        [TestMethod]
        public void ValidXpathDoesNotFailBackToCssSelector()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("html.Extract //h3");
            var result =
                pipeline.Execute(
                    "<html><head></head><body><h3 class='title'>2 + 2 = 10 ....in base 4</h3></body></html>");
            Assert.AreEqual("2 + 2 = 10 ....in base 4", result);
        }

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