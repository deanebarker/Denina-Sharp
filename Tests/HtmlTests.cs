using System;
using DeninaSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class HtmlTests
    {
        [TestMethod]
        public void Wrap()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("html.Wrap -tag:p -class:theClass -id:theId");
            string result = pipeline.Execute("Deane");

            Assert.AreEqual("<p class=\"theClass\" id=\"theId\">Deane</p>", result);
        }

        [TestMethod]
        public void LineBreaks()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Html.LineBreaks");
            var result = pipeline.Execute("Deane" + Environment.NewLine + "Annie");

            Assert.AreEqual("Deane<br/>Annie", result);
        }
    }
}