using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlendInteractive.TextFilterPipeline.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class FileSystemFilterTests
    {
        [TestMethod]
        public void ReadContentFromFile()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("file.Read Utility/text.txt");
            var result = pipeline.Execute(String.Empty);

            Assert.AreEqual("Deane", result);
        }
    }
}
