using System.Collections.Generic;
using System.Linq;
using BlendInteractive.TextFilterPipeline.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class CommandParsing
    {
        [TestMethod]
        public void PopulateCommand()
        {
            var command = new TextFilterCommand();
            command.CommandName = "Replace";
            command.CommandArgs.Add("1", "a");
            command.CommandArgs.Add("2", "b");

            Assert.AreEqual("replace", command.CommandName);
            Assert.AreEqual("a", command.CommandArgs.First().Value);
            Assert.AreEqual("b", command.CommandArgs.Last().Value);
        }

        [TestMethod]
        public void ParseCommandString()
        {
            var command = new TextFilterCommand("Replace a b");

            Assert.AreEqual("replace", command.CommandName);
            Assert.AreEqual("a", command.CommandArgs.First().Value);
            Assert.AreEqual("b", command.CommandArgs.Last().Value);
        }

        [TestMethod]
        public void AddCommandByObject()
        {
            var command = new TextFilterCommand();
            command.CommandName = "Replace";
            command.CommandArgs.Add("a", "b");

            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand(command);

            Assert.AreEqual(1, pipeline.Commands.Count());
            Assert.AreEqual("replace", pipeline.Commands.First().CommandName);
            Assert.AreEqual("b", pipeline.Commands.First().CommandArgs.First().Value);
        }


        [TestMethod]
        public void AddCommandByString()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("Replace", new Dictionary<object, string> {{"a", "b"}});

            Assert.AreEqual("b", pipeline.Commands.First().CommandArgs.Last().Value);
        }

        [TestMethod]
        public void AddQuotedCommandsByString()
        {
            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("Replace Deane \"Annie was here\"");

            Assert.AreEqual("replace", pipeline.Commands.First().CommandName);
            Assert.AreEqual("Deane", pipeline.Commands.First().CommandArgs.First().Value);
            Assert.AreEqual("Annie was here", pipeline.Commands.First().CommandArgs.Last().Value);
        }

        [TestMethod]
        public void AddMultipleCommandsByString()
        {
            string commandString = @"
                Replace a b
                Replace c d
            ";

            var pipeline = new TextFilterPipeline();
            pipeline.AddCommands(commandString);

            Assert.AreEqual("replace", pipeline.Commands.First().CommandName);
            Assert.AreEqual("replace", pipeline.Commands[1].CommandName);
            Assert.AreEqual("b", pipeline.Commands.First().CommandArgs.Last().Value);
        }

        [TestMethod]
        public void AddCommandWithComment()
        {
            string commandString = @"
                Replace a b
                #Replace c d
            ";

            var pipeline = new TextFilterPipeline();
            pipeline.AddCommands(commandString);

            Assert.AreEqual(1, pipeline.Commands.Count);          
        }

        [TestMethod]
        public void AddCommandWithEmptyLines()
        {
            string commandString = @"
                Replace a b
                



                Replace c d
            ";

            var pipeline = new TextFilterPipeline();
            pipeline.AddCommands(commandString);

            Assert.AreEqual(2, pipeline.Commands.Count);
        }
    }
}