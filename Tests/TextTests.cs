using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlendInteractive.TextFilterPipeline.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class TextTests
    {
        [TestMethod]
        public void Replace()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("Text.Replace Deane Annie");
            string result = pipeline.Execute("Deane was here.");

            Assert.AreEqual("Annie was here.", result);
        }


        [TestMethod]
        public void ReplaceAll()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("Text.ReplaceAll deane");
            string result = pipeline.Execute("Annie was here.");

            Assert.AreEqual("deane", result);
        }

        [TestMethod]
        public void Format()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("Text.Format \"{0} was here.\"");
            string result = pipeline.Execute("Deane");

            Assert.AreEqual("Deane was here.", result);
        }

        [TestMethod]
        public void FormatFromVariables()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("WriteTo $Name");
            pipeline.AddCommand("Text.Format \"{Name} was here.\"");
            string result = pipeline.Execute("Deane");

            Assert.AreEqual("Deane was here.", result);
        }

        [TestMethod]
        public void Append()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("Text.Append Deane");
            string result = pipeline.Execute("I am ");

            Assert.AreEqual("I am Deane", result);
        }

        [TestMethod]
        public void Prepend()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("Text.Prepend Deane");
            string result = pipeline.Execute(" was here.");

            Assert.AreEqual("Deane was here.", result);
        }
    }
}
