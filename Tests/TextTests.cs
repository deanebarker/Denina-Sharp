using System;
using DeninaSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class TextTests
    {
        [TestMethod]
        public void Replace()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.Replace Deane Annie");
            string result = pipeline.Execute("Deane was here.");

            Assert.AreEqual("Annie was here.", result);
        }


        [TestMethod]
        public void ReplaceAll()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.ReplaceAll deane");
            string result = pipeline.Execute("Annie was here.");

            Assert.AreEqual("deane", result);
        }

        [TestMethod]
        public void Format()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.Format \"{0} was here.\"");
            string result = pipeline.Execute("Deane");

            Assert.AreEqual("Deane was here.", result);
        }

        [TestMethod]
        public void FormatFromVariables()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("WriteTo $Name");
            pipeline.AddCommand("Text.Format \"{Name} was here.\"");
            string result = pipeline.Execute("Deane");

            Assert.AreEqual("Deane was here.", result);
        }

        [TestMethod]
        public void Append()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.Append Deane");
            string result = pipeline.Execute("I am ");

            Assert.AreEqual("I am Deane", result);
        }

        [TestMethod]
        public void Prepend()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.Prepend Deane");
            string result = pipeline.Execute(" was here.");

            Assert.AreEqual("Deane was here.", result);
        }

        [TestMethod]
        public void ExtractRegex()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.ExtractRegex e(..)e");
            
            string result = pipeline.Execute("Deane");
            Assert.AreEqual(result, "an");

            result = pipeline.Execute("Annie");
            Assert.AreEqual(String.Empty, result);
        }
    }
}
