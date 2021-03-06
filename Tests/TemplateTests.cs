﻿using DeninaSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class TemplateTests : BaseTests
    {
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
        public void LoopingNodesAsIndexable()
        {
            var template = "Foo {% for thing in data.list['//person'] %}{{ thing.name }} {% endfor %}";
            var pipeline = new Pipeline();
            pipeline.SetVariable("__template", template);
            pipeline.AddCommand("Template.FromXml -template:$__template");
            string result = pipeline.Execute("<root><person><name>Bar</name></person><person><name>Baz</name></person></root>");

            Assert.AreEqual("Foo Bar Baz ", result);
        }

        [TestMethod]
        public void LoopingNodesAsShorthand()
        {
            var template = "Foo {% for thing in data.people.list-person %}{{ thing.name }} {% endfor %}";
            var pipeline = new Pipeline();
            pipeline.SetVariable("__template", template);
            pipeline.AddCommand("Template.FromXml -template:$__template");
            string result = pipeline.Execute("<root><people><person><name>Bar</name></person><person><name>Baz</name></person></people><more-people><person><name>This should not be iterated.</name></person></more-people></root>");

            Assert.AreEqual("Foo Bar Baz ", result);
        }

        // This does not pass. I do not know how to fix this one.
        /* <root>
         *   <person>Deane</person>
         *   <person>Annie</person>
         * </root>
         * 
         * {% for person in data %} will not work, because we never call a method. "data" is the drop. There's no method call we can catch here.
        */    
        //[TestMethod]
        //public void LoopingstDirectNodesWithDirectText()
        //{
        //    var template = "{% for person in data %}{{ person }} {% endfor %}";
        //    var pipeline = new Pipeline();
        //    pipeline.SetVariable("__template", template);
        //    pipeline.AddCommand("Template.FromXml -template:$__template");
        //    string result = pipeline.Execute("<people><person>Deane</person><person>Annie</person></people>");

        //    Assert.AreEqual("Deane Annie ", result);
        //}

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
        public void AttributeShorthand()
        {
            var template = "Foo {{ data.name.attr-last }}";
            var pipeline = new Pipeline();
            pipeline.SetVariable("__template", template);
            pipeline.AddCommand("Template.FromXml -template:$__template");
            string result = pipeline.Execute("<person><name first=\"Foo\" last=\"Bar\"></name></person>");

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


        [TestMethod]
        public void NumberFormatting()
        {
            var template = "{{ vars.age|number:'0.00' }}";
            var pipeline = new Pipeline();
            pipeline.SetVariable("__template", template);
            pipeline.SetVariable("age", 47);
            pipeline.AddCommand("Template.FromText -template:$__template");
            string result = pipeline.Execute();

            Assert.AreEqual("47.00", result);
        }

        [TestMethod]
        public void InvalidNumberFormatting()
        {
            var template = "{{ vars.age|number:'0.00' }}";
            var pipeline = new Pipeline();
            pipeline.SetVariable("__template", template);
            pipeline.SetVariable("age", "this is not an age");
            pipeline.AddCommand("Template.FromText -template:$__template");
            string result = pipeline.Execute();

            Assert.AreEqual("this is not an age", result);
        }

        [TestMethod]
        public void PluralizeFilter()
        {
            var template = "{{ vars.numbersingle|pluralize:'single','plural' }} {{ vars.numberplural|pluralize:'single','plural' }} {{ vars.collectionsingle|pluralize:'single','plural' }} {{ vars.collectionplural|pluralize:'single','plural' }}";
            var pipeline = new Pipeline();
            pipeline.SetVariable("__template", template);
            pipeline.SetVariable("numbersingle", 1);
            pipeline.SetVariable("numberplural", 2);
            pipeline.SetVariable("collectionsingle", new string[] { "" });
            pipeline.SetVariable("collectionplural", new string[] { "", "" });
            pipeline.AddCommand("Template.FromText -template:$__template");
            string result = pipeline.Execute();

            Assert.AreEqual("single plural single plural", result);
        }
    }
}
