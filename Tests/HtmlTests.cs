using System;
using DeninaSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class HtmlTests : BaseTests
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

        [TestMethod]
        public void Extract()
        {
            var innerText = "Deane was <b>here</b>";

            var pipeline = new Pipeline();
            pipeline.AddCommand(@"Html.Extract -path:p.para");
            var result = pipeline.Execute("<p class=\"para\">" + innerText + "</p>");

            Assert.AreEqual(innerText, result);
        }

        [TestMethod]
        public void StripTags()
        {
            var innerText = "Deane was <b>here</b>";

            var pipeline = new Pipeline();
            pipeline.AddCommand(@"Html.Strip");
            var result = pipeline.Execute(innerText);

            Assert.AreEqual("Deane was here", result);
        }

        [TestMethod]
        public void SetAttribute()
        {
            var html = "<div id=\"james-bond\" data-number=\"006\"></div>";

            var pipeline = new Pipeline();
            pipeline.AddCommand("Html.SetAttribute -path:div#james-bond -attribute:data-number -val:007");
            var result = pipeline.Execute(html);

            Assert.IsTrue(result.Contains("007"));
        }
    }
}