using DeninaSharp.Core;
using DeninaSharp.Core.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    [TestClass]
    public class FileTests
    {
        [TestInitialize]
        public void Init()
        {
            Pipeline.SetGlobalVariable(File.SANDBOX_VARIABLE_NAME, AppDomain.CurrentDomain.BaseDirectory);
        }

        [TestMethod]
        public void ReadContentFromFile()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("file.Read -file:Utility/text.txt");
            string result = pipeline.Execute(String.Empty);

            Assert.AreEqual("Deane", result);
        }
    }
}