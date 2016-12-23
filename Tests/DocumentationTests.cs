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
    public class DocumentationTests
    {
        [TestInitialize]
        public void Init()
        {
            Pipeline.Init();
        }

        [TestMethod]
        public void LoadsForCategory()
        {
            Assert.AreEqual(Pipeline.CommandMethods.Count(m => m.Key.StartsWith("xml.")), Pipeline.FilterDoc.Count(d => d.Key.StartsWith("xml.")));
        }
    }
}
