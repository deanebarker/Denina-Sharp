using System;
using System.Linq;
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

        [TestMethod]
        public void ResolveVariableNames()
        {
            var input = "Deane";
            
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("WriteTo myVar");   // Write the input to a variable
            pipeline.AddCommand("Prepend $myVar");  // Prepend that variable onto the input
            var result = pipeline.Execute(input);

            Assert.AreEqual(String.Concat(input, input), result);   // Result should be the input twice
        }

        [TestMethod]
        public void VariablePrefixes()
        {
            var input = "Deane";
            var variableName = "myVar";

            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("WriteTo " + variableName);
            pipeline.AddCommand("WriteTo $" + variableName);    // The prefix should be removed. This should write the same place as the first command.
            var result = pipeline.Execute(input);

            Assert.AreEqual(1, pipeline.Variables.Count);
            Assert.AreEqual(variableName, pipeline.Variables.First().Key);
            Assert.AreEqual(input, pipeline.GetVariable(variableName));
        }
    }
}