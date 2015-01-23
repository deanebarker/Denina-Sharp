using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using BlendInteractive.TextFilterPipeline.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class Core
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
            pipeline.AddCommand("Replace", new Dictionary<string, string>() {{"a", "b"}});

            Assert.AreEqual("b", pipeline.Commands.First().CommandArgs.Last().Value);
        }

        [TestMethod]
        public void AddMultipleCommandsByString()
        {
            var commandString = @"
                Replace a b
                Replace c d
            ";

            var pipeline = new TextFilterPipeline();
            pipeline.AddCommands(commandString);

            Assert.AreEqual("replace", pipeline.Commands.First().CommandName);
            Assert.AreEqual("replace", pipeline.Commands[1].CommandName);
            Assert.AreEqual("b", pipeline.Commands.First().CommandArgs.Last().Value);
        }
    }
}
