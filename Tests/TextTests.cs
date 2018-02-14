using System;
using System.Runtime;
using DeninaSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Tests
{
    [TestClass]
    public class TextTests : BaseTests
    {
        [TestMethod]
        public void Replace()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.Replace -old:Deane -new:Annie");
            string result = pipeline.Execute("Deane was here.");
            Assert.AreEqual("Annie was here.", result);

            pipeline = new Pipeline();
            pipeline.AddCommand("Text.Replace -old:deane -new:Annie -case:true");
            result = pipeline.Execute("Deane was here.");
            Assert.AreEqual("Deane was here.", result);  // Should not replace, since we're respecting case
        }


        [TestMethod]
        public void ReplaceAll()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.ReplaceAll -text:deane");
            string result = pipeline.Execute("Annie was here.");

            Assert.AreEqual("deane", result);
        }

        [TestMethod]
        public void Format()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.Format -template:\"{0} was here.\"");
            string result = pipeline.Execute("Deane");

            Assert.AreEqual("Deane was here.", result);
        }

        [TestMethod]
        public void FormatFromVariables()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("WriteTo Name");
            pipeline.AddCommand("Text.Format -template:\"{Name} was here.\"");
            string result = pipeline.Execute("Deane");

            Assert.AreEqual("Deane was here.", result);
        }

        [TestMethod]
        public void Append()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.Append -suffix:Deane");
            string result = pipeline.Execute("I am ");

            Assert.AreEqual("I am Deane", result);
        }

        [TestMethod]
        public void Prepend()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.Prepend -prefix:Deane");
            string result = pipeline.Execute(" was here.");

            Assert.AreEqual("Deane was here.", result);
        }

        [TestMethod]
        public void ExtractRegex()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.ExtractRegex -pattern:e(..)e");
            
            string result = pipeline.Execute("Deane");
            Assert.AreEqual(result, "an");

            result = pipeline.Execute("Annie");
            Assert.AreEqual(String.Empty, result);
        }

        [TestMethod]
        public void FormatLines()
        {
            var input = new[]
            {
                "Deane",
                "Barker"
            };

            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.FormatLines -template:({0})");
            var result = pipeline.Execute(String.Join(Environment.NewLine, input));

            Assert.AreEqual(result, "(Deane)(Barker)");

        }
    }
}
