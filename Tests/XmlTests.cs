using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlendInteractive.TextFilterPipeline.Core;
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

            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("File.Read utility/data.xml => $xml");
            pipeline.AddCommand("File.Read utility/transform.xslt => $xsl");
            pipeline.AddCommand("Xml.TransformXml $xsl $xml => $result");
            pipeline.AddCommand("$result =>");

            var result = pipeline.Execute();

            Assert.IsTrue(result.EndsWith("Deane"));
        }
    }
}
