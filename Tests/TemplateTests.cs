using DeninaSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class TemplateTests
    {
        [TestInitialize]
        public void Init()
        {
            Pipeline.Init();
        }

        [TestMethod]
        public void FromText()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Template.FromText -template:\"Foo {{ data }}\"");
            string result = pipeline.Execute("Bar");

            Assert.AreEqual("Foo Bar", result);
        }

        [TestMethod]
        public void FromXml()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Template.FromXml -template:\"Foo {{ data.name.first }} {{ data.name.last }}\"");
            string result = pipeline.Execute("<person><name><first>Bar</first><last>Baz</last></name></person>");

            Assert.AreEqual("Foo Bar Baz", result);
        }

        [TestMethod]
        public void FromXmlNodes()
        {
            var template = "Foo {% for thing in data.person %}{{ thing.name }} {% endfor %}";
            var pipeline = new Pipeline();
            pipeline.SetVariable("__template", template);
            pipeline.AddCommand("Template.FromXml -template:$__template");
            string result = pipeline.Execute("<root><person><name>Bar</name></person><person><name>Baz</name></person></root>");

            Assert.AreEqual("Foo Bar Baz ", result);
        }

        [TestMethod]
        public void Variables()
        {
            var template = "Foo {{ vars.bar }}";
            var pipeline = new Pipeline();
            pipeline.SetVariable("bar","Bar");
            pipeline.SetVariable("__template",template);
            pipeline.AddCommand("Template.FromText -template:$__template");
            string result = pipeline.Execute(string.Empty);

            Assert.AreEqual("Foo Bar", result);
        }

        [TestMethod]
        public void XPathQuery()
        {
            var template = "Foo {{ data.xpath['/person/name/@last'] }}";
            var pipeline = new Pipeline();
            pipeline.SetVariable("__template", template);
            pipeline.AddCommand("Template.FromXml -template:$__template");
            string result = pipeline.Execute("<person><name first=\"Foo\" last=\"Bar\"></name></person>");

            Assert.AreEqual("Foo Bar", result);
        }
    }
}
