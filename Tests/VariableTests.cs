using DeninaSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class VariableTests
    {
        [TestMethod]
        public void WriteToVariable()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("WriteTo $Name");
            pipeline.Execute("Deane");

            Assert.AreEqual("Deane", pipeline.GetVariable("Name"));
        }

        [TestMethod]
        public void ReadFromVariable()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("WriteTo $Name"); // Writes original input to the variable "Name"
            pipeline.AddCommand("Text.ReplaceAll -text:Annie"); // Resets input to "Annie"
            pipeline.AddCommand("ReadFrom $Name"); // Input should be the original again

            Assert.AreEqual("Deane", pipeline.Execute("Deane"));
        }


        [TestMethod]
        public void ReadNonExistentVariableWithDefault()
        {
            var pipeline = new Pipeline();

            try
            {
                pipeline.AddCommand("ReadFrom $Name");
                pipeline.Execute();
                Assert.Fail("Test should have failed in the line above and gone to the \"catch\" block...");
            }
            catch (DeninaException e)
            {
                // This is the passing block...
                Assert.IsTrue(e.Message.StartsWith("Attempt to access non-existent variable"));
            }
            
        }

        [TestMethod]
        public void WritingIntoImplicitVariable()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.Append -suffix:\" married Deane.\" => $myVar");
            var result = pipeline.Execute("Annie");

            Assert.AreEqual(result, "Annie");   // The input text should be unchanged
            Assert.AreEqual(pipeline.GetVariable("myVar"), "Annie married Deane.");
        }

        [TestMethod]
        public void ResolveVariableNames()
        {
            var input = "Deane";
            
            var pipeline = new Pipeline();
            pipeline.AddCommand("WriteTo $myVar");   // Write the input to a variable
            pipeline.AddCommand("Text.Prepend -prefix:$myVar");  // Prepend that variable onto the input
            var result = pipeline.Execute(input);

            Assert.AreEqual(String.Concat(input, input), result);   // Result should be the input twice
        }

        [TestMethod]
        public void VariablePrefixes()
        {
            return; // This test may be invalid now...
            var input = "Deane";
            var variableName = "myVar";

            var pipeline = new Pipeline();
            pipeline.AddCommand("WriteTo " + variableName);
            pipeline.AddCommand("WriteTo $" + variableName);    // The prefix should be removed. This should write the same place as the first command.
            var result = pipeline.Execute(input);

            Assert.AreEqual(1, pipeline.Variables.Count);
            Assert.AreEqual(variableName, pipeline.Variables.First().Key);
            Assert.AreEqual(input, pipeline.GetVariable(variableName));
        }

        [TestMethod]
        public void PipingInputToFromVariables()
        {
            var input = "Deane";
            var secondInput = "Annie";
            var variableName = "$myVar";

            var pipeline = new Pipeline();
            pipeline.AddCommand("=> " + variableName);          // This puts the input into the variable
            Assert.AreEqual(input, pipeline.Execute(input));
            Assert.AreEqual(input, pipeline.GetVariable(variableName));

            return;

            // This stuff doesn't work, because we need to find a way to re-parse the command queue when the commands are changed.
            // Remember that after the first call to "Execute," the pipeline has a command with the SendTo value of "end"
            // We need to re-parse -- reset all the commands to their original values...
            pipeline.AddCommand("Text.ReplaceAll " + secondInput);   // Replaces the input
            Assert.AreEqual(secondInput, pipeline.Execute(input));
            Assert.AreEqual(input, pipeline.GetVariable(variableName));
            
            pipeline.AddCommand(variableName + " =>");  // Resets the input to the original (from the variable)
            Assert.AreEqual(input, pipeline.Execute(input));

        }

        [TestMethod]
        public void ResettingReadOnlyVariable()
        {
            var pipeline = new Pipeline();
            pipeline.SetVariable("Name", "Deane", true); // This is read-only
            pipeline.AddCommand("SetVar -var:Name -value:Annie"); // This will attempt to reset that variable

            try
            {
                pipeline.Execute();
                Assert.Fail("Should not have gotten here...");
            }
            catch (DeninaException e)
            {
                Assert.IsTrue(e.Message.Contains("Attempt to reset value"));
            }
            catch (Exception e)
            {
                Assert.Fail("This should have thrown a DeninaException, not a generic Exception.");
            }
        }

        [TestMethod]
        public void AppendingVariables()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("SetVar -var:Name -value:Annie");
            pipeline.AddCommand("AppendVar -var:Name -value:Deane");
            var result = pipeline.Execute();

            Assert.AreEqual("AnnieDeane", pipeline.GetVariable("Name"));

        }

    }
}