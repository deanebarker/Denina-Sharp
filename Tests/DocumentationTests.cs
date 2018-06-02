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
            Assert.AreEqual(Pipeline.CommandMethods.Count(m => m.Key.StartsWith("Xml.")), Pipeline.FilterDoc.Count(d => d.Key.StartsWith("Xml.")));
        }
    }
}
