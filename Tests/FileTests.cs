using System;
using BlendInteractive.TextFilterPipeline.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class FileTests
    {
        [TestMethod]
        public void ReadContentFromFile()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("file.Read Utility/text.txt");
            string result = pipeline.Execute(String.Empty);

            Assert.AreEqual("Deane", result);
        }
    }
}