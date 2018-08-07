using System;
using System.Collections.Generic;
using DeninaSharp.Core;
using DeninaSharp.Core.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class CommandFactoryTests : BaseTests
    {
        [TestMethod]
        public void SimpleFactory()
        {
            Pipeline.RegisterCommandFactory("Core.Include", (command) => { return PipelineCommandParser.ParseCommandString("Text.Append -suffix:Bar"); });

            var p = new Pipeline();
            p.AddCommand("Text.Append -suffix:Foo");
            p.AddCommand("Include");
            p.AddCommand("Text.Append -suffix:Baz");

            var result = p.Execute();

            Assert.AreEqual("FooBarBaz", result);
        }

        [TestMethod]
        public void NestedInclude()
        {
            Pipeline.RegisterCommandFactory("Core.Include", (command) =>
            {
                // The first incl;uded script references a second script
                if (command.GetArgument("key") == "first")
                {
                    return PipelineCommandParser.ParseCommandString("Text.Append -suffix:Bar1").Concat(PipelineCommandParser.ParseCommandString("Core.Include -key:second"));
                }

                // This is the second script
                return PipelineCommandParser.ParseCommandString("Text.Append -suffix:Bar2");
            });

            var p = new Pipeline();
            p.AddCommand("Text.Append -suffix:Foo");
            p.AddCommand("Include -key:first");
            p.AddCommand("Text.Append -suffix:Baz");

            var result = p.Execute();

            Assert.AreEqual("FooBar1Bar2Baz", result);
        }

        [TestMethod]
        public void NullFactory()
        {
            Pipeline.RegisterCommandFactory("Core.Include", (command) =>
            {
                // This is an empty list...
                return new List<PipelineCommand>();
            });

            var p = new Pipeline();
            p.AddCommand("Text.Append -suffix:Foo");
            p.AddCommand("Include"); // This should include nothing, and just get removed
            p.AddCommand("Text.Append -suffix:Baz");

            var result = p.Execute();

            Assert.AreEqual("FooBaz", result);
        }

        [TestMethod]
        public void SubstitutedCommand()
        {
            Pipeline.RegisterCommandFactory("Add.IsAwesome", (command) =>
            {
                // The first included script references a second script
                if (command.NormalizedCommandName.ToLower() == "Add.IsAwesome".ToLower())
                {
                    return PipelineCommandParser.ParseCommandString("Text.Append -suffix:IsAwesome");
                }
                else
                {
                    return new List<PipelineCommand>();
                }
            });

            var p = new Pipeline();
            p.AddCommand("Text.Append -suffix:Foo");
            p.AddCommand("Add.IsAwesome");
            p.AddCommand("Text.Append -suffix:Bar");

            var result = p.Execute();

            Assert.AreEqual("FooIsAwesomeBar", result);
        }

        [TestMethod]
        public void RegexFactoryCommand()
        {
            Func<PipelineCommand, IEnumerable<PipelineCommand>> commandFactory = (command) =>
             {
                 command.FullyQualifiedCommandName = "Text.Append";
                 command.CommandArgs.Add("suffix", command.OriginalText); // Returns the command name

                return new List<PipelineCommand>() { command };
             };

            Pipeline p;

            // Test EndsWith
            Pipeline.ClearCommandFactories();
            Pipeline.RegisterCommandFactory("Deane.*", commandFactory);
            p = new Pipeline();
            p.AddCommand("Deane.IsAwesome");
            Assert.AreEqual("Deane.IsAwesome", p.Execute());

            // Test single-character wildcard
            Pipeline.ClearCommandFactories();
            Pipeline.RegisterCommandFactory("De?ne.IsAwesome*", commandFactory);
            p = new Pipeline();
            p.AddCommand("Deane.IsAwesome");
            Assert.AreEqual("Deane.IsAwesome", p.Execute());

            // Test StartsWith
            Pipeline.ClearCommandFactories();
            Pipeline.RegisterCommandFactory("*.IsAwesome", commandFactory);
            p = new Pipeline();
            p.AddCommand("Deane.IsAwesome");
            Assert.AreEqual("Deane.IsAwesome", p.Execute());

            // Test multi-character wildcard
            Pipeline.ClearCommandFactories();
            Pipeline.RegisterCommandFactory("Deane.*Awesome", commandFactory);
            p = new Pipeline();
            p.AddCommand("Deane.IsAwesome");
            Assert.AreEqual("Deane.IsAwesome", p.Execute());
        }

        [TestMethod]
        public void Logging()
        {
            Pipeline.RegisterCommandFactory("*", (command) =>
            {
                return PipelineCommandParser.ParseCommandString("Text.Prepend -prefix:doesntmatter");
            });

            var p = new Pipeline();
            p.AddCommand("Text.Append -suffx:doesntmatter");
            p.Execute();

            Assert.AreEqual(p.LogEntries.First().CommandFactorySource, "Text.Append -suffx:doesntmatter");
        }
    }
}
