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
    public class XmlFilterTests
    {
        [TestMethod]
        public void TransformXml()
        {
            // This test relies on a controled XSLT in the "Utility" folder

            var elementValue = "Deane";

            var pipeline = new TextFilterPipeline();

            // Write the incoming XML to a variable
            pipeline.AddCommand("WriteTo xml");

            // Read in the XSL
            pipeline.AddCommand("File.Read utility/transform.xslt");
            pipeline.AddCommand("WriteTo xsl");

            // Read back the XML, then call the transform from the XSL in the variable
            pipeline.AddCommand("ReadFrom xml");
            pipeline.AddCommand("XML.TransformXmlFromVariable xsl");

            var result = pipeline.Execute("<element>" + elementValue + "</element>");

            Assert.AreEqual(result.Substring(result.Length - elementValue.Length), elementValue);
        }
    }
}
