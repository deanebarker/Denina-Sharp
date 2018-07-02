using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeninaSharp.Core.Documentation;
using DeninaSharp.Core;
using System.Linq;
using System.Reflection;
using System.IO;

namespace Tests
{
    [TestClass]
    public class DocumentationTests : BaseTests
    {
        [TestMethod]
        public void LoadsForCategory()
        {
            Assert.AreEqual(Pipeline.CommandMethods.Count(m => m.Key.StartsWith("Xml.")), Pipeline.FilterDocs.Count(d => d.Key.StartsWith("Xml.")));
        }

        [TestMethod]
        public void FilterDocLoadedEvent()
        {
            Pipeline.FilterDocLoaded += (s, e) =>
            {
                e.FilterDoc.Name = "Foo";
            };
            Pipeline.Init();

            Assert.AreEqual("Foo", Pipeline.FilterDocs.First().Value.Name);
        }

        [TestMethod]
        public void CategoryDocLoadedEvent()
        {
            Pipeline.CategoryDocLoaded += (s, e) =>
            {
                e.CategoryDoc.Name = "Foo";
            };
            Pipeline.Init();

            Assert.AreEqual("Foo", Pipeline.CategoryDocs.First().Value.Name);
        }
    }
}
