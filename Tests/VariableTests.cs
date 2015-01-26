using BlendInteractive.TextFilterPipeline.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class VariableTests
    {
        [TestMethod]
        public void WriteToVariable()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("WriteTo Name");
            pipeline.Execute("Deane");

            Assert.AreEqual("Deane", pipeline.Variables["Name"]);
        }

        [TestMethod]
        public void ReadFromVariable()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("WriteTo Name"); // Writes original input to the variable "Name"
            pipeline.AddCommand("ReplaceAll Annie"); // Resets input to "Annie"

            Assert.AreEqual("Annie", pipeline.Execute("Anything"));

            pipeline.AddCommand("ReadFrom Name"); // Input should be the original again

            Assert.AreEqual("Deane", pipeline.Execute("Deane"));
        }


        [TestMethod]
        public void ReadNonExistentVariableWithDefault()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("ReadFrom Name Deane");

            Assert.AreEqual("Deane", pipeline.Execute("Annie"));
        }

        [TestMethod]
        public void WritingIntoImplicitVariable()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("Append \" married Deane.\" => myVar");
            var result = pipeline.Execute("Annie");

            Assert.AreEqual(result, "Annie");   // The input text should be unchanged
            Assert.AreEqual(pipeline.Variables["myVar"], "Annie married Deane.");
        }
    }
}