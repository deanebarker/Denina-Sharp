using System;
using DeninaSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class XmlTests
    {
        [TestMethod]
        public void TransformXml()
        {
            // This test relies on controlled XSLT/XML files in the "Utility" folder
            Pipeline.SetFileSandbox(AppDomain.CurrentDomain.BaseDirectory);

            var pipeline = new Pipeline();
            pipeline.AddCommand("File.Read utility/data.xml => $xml");
            pipeline.AddCommand("File.Read utility/transform.xslt => $xsl");
            pipeline.AddCommand("Xml.Transform $xsl $xml => $result");
            pipeline.AddCommand("$result =>");

            var result = pipeline.Execute();

            Assert.IsTrue(result.EndsWith("Deane"));
        }

        [TestMethod]
        public void FormatNodes()
        {
            Pipeline.SetFileSandbox(AppDomain.CurrentDomain.BaseDirectory);


            var pipeline = new Pipeline();
            pipeline.AddCommand("File.Read utility/data.xml");
            pipeline.AddCommand("Xml.FormatNodes //element \"Value: {.}\"");
            var result = pipeline.Execute();

            Assert.AreEqual(result, "Value: Deane");
        }
    }
}
