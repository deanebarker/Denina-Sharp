using DeninaSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    [TestClass]
    public class FileTests
    {
        [TestMethod]
        public void ReadContentFromFile()
        {
            Pipeline.SetFileSandbox(AppDomain.CurrentDomain.BaseDirectory);

            var pipeline = new Pipeline();
            pipeline.AddCommand("file.Read Utility/text.txt");
            string result = pipeline.Execute(String.Empty);

            Assert.AreEqual("Deane", result);
        }
    }
}