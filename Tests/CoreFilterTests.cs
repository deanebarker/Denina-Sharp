using BlendInteractive.TextFilterPipeline.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class CoreFilterTests
    {
        [TestMethod]
        public void Replace()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("Replace Deane Annie");
            string result = pipeline.Execute("Deane was here.");

            Assert.AreEqual("Annie was here.", result);
        }


        [TestMethod]
        public void ReplaceAll()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("ReplaceAll deane");
            string result = pipeline.Execute("Annie was here.");

            Assert.AreEqual("deane", result);
        }

        [TestMethod]
        public void Format()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("Format \"{0} was here.\"");
            string result = pipeline.Execute("Deane");

            Assert.AreEqual("Deane was here.", result);
        }

        [TestMethod]
        public void FormatFromVariables()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("WriteTo Name");
            pipeline.AddCommand("Format \"{Name} was here.\"");
            string result = pipeline.Execute("Deane");

            Assert.AreEqual("Deane was here.", result);
        }

        [TestMethod]
        public void Append()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("Append Deane");
            string result = pipeline.Execute("I am ");

            Assert.AreEqual("I am Deane", result);
        }

        [TestMethod]
        public void Prepend()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("Prepend Deane");
            string result = pipeline.Execute(" was here.");

            Assert.AreEqual("Deane was here.", result);
        }
    }
}