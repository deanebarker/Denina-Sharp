using System.Runtime;
using DeninaSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class CommandParsing : BaseTests
    {
        [TestMethod]
        public void PopulateCommand()
        {
            var command = new PipelineCommand();
            command.CommandName = "Replace";
            command.CommandArgs.Add("1", "a");
            command.CommandArgs.Add("2", "b");

            Assert.AreEqual("Replace", command.CommandName);
            Assert.AreEqual("core.replace", command.NormalizedCommandName);
            Assert.AreEqual("a", command.CommandArgs.First().Value);
            Assert.AreEqual("b", command.CommandArgs.Last().Value);
        }

        [TestMethod]
        public void AddCommandInConstructor()
        {
            var pipeline = new Pipeline("Prepend FOO");

            Assert.AreEqual(1, pipeline.Commands.Count);
            Assert.AreEqual("Prepend", pipeline.Commands.First().CommandName);
        }

        [TestMethod]
        public void AddCommandByObject()
        {
            var command = new PipelineCommand();
            command.CommandName = "Replace";
            command.CommandArgs.Add("a", "b");

            var pipeline = new Pipeline();
            pipeline.AddCommand(command);

            Assert.AreEqual(1, pipeline.Commands.Count());
            Assert.AreEqual("Replace", pipeline.Commands.First().CommandName);
            Assert.AreEqual("b", pipeline.Commands.First().CommandArgs.First().Value);
        }


        [TestMethod]
        public void AddCommandByString()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Replace", new Dictionary<object, string> {{"a", "b"}});

            Assert.AreEqual("b", pipeline.Commands.First().CommandArgs.Last().Value);
        }

        [TestMethod]
        public void AddQuotedCommandsByString()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Replace -old:Deane -new:\"Annie was here\"");

            Assert.AreEqual("Replace", pipeline.Commands.First().CommandName);
            Assert.AreEqual("Deane", pipeline.Commands.First().CommandArgs.First().Value);
            Assert.AreEqual("Annie was here", pipeline.Commands.First().CommandArgs.Last().Value);
        }

        [TestMethod]
        public void AddMultipleCommandsByString()
        {
            // These have to be flush with the left margin to void whitespace
            string commandString = @"
Text.Replace -old:a -new:b
Text.Replace -old:c -new:d
            ";

            var pipeline = new Pipeline();
            pipeline.AddCommand(commandString);

            Assert.AreEqual("Text.Replace", pipeline.Commands.First().CommandName);
            Assert.AreEqual("Text.Replace", pipeline.Commands[1].CommandName);
            Assert.AreEqual("b", pipeline.Commands.First().CommandArgs.Last().Value);
        }

        [TestMethod]
        public void AddCommandWithComment()
        {
            string commandString = @"
                Replace -old:a -new:b
                #Replace -old:c -new:d
            ";

            var pipeline = new Pipeline();
            pipeline.AddCommand(commandString);

            Assert.AreEqual(1, pipeline.Commands.Count);
        }

        [TestMethod]
        public void AddCommandWithEmptyLines()
        {
            string commandString = @"
                Replace a b
                



                Replace c d
            ";

            var pipeline = new Pipeline();
            pipeline.AddCommand(commandString);

            Assert.AreEqual(2, pipeline.Commands.Count);
        }

        [TestMethod]
        public void AddingIdenticalArgumentKeys()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.Format -template:deane -template:barker -template:was -template:here -other:argument");

            Assert.AreEqual(5, pipeline.Commands.First().CommandArgs.Count);
            Assert.AreEqual("template.3", pipeline.Commands.First().CommandArgs.Skip(3).First().Key);
            Assert.AreEqual(4, pipeline.Commands.First().GetMultiArgument("template").Count);
        }
    }
}