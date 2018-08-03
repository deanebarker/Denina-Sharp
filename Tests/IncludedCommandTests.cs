using System;
using System.Collections.Generic;
using DeninaSharp.Core;
using DeninaSharp.Core.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class IncludedCommandTests : BaseTests
    {
        [TestMethod]
        public void SimpleInclude()
        {
            Pipeline.CommandIncluder = (command) => { return PipelineCommandParser.ParseCommandString("Text.Append -suffix:Bar"); };

            var p = new Pipeline();
            p.AddCommand("Text.Append -suffix:Foo");
            p.AddCommand("Include -key:whatever");
            p.AddCommand("Text.Append -suffix:Baz");

            var result = p.Execute();

            Assert.AreEqual("FooBarBaz", result);
        }

        [TestMethod]
        public void NestedInclude()
        {
            Pipeline.CommandIncluder = (command) =>
            {
                // The first incl;uded script references a second script
                if (command.GetArgument("key") == "first")
                {
                    return PipelineCommandParser.ParseCommandString("Text.Append -suffix:Bar1").Concat(PipelineCommandParser.ParseCommandString("Core.Include -key:second"));
                }

                // This is the second script
                return PipelineCommandParser.ParseCommandString("Text.Append -suffix:Bar2");
            };

            var p = new Pipeline();
            p.AddCommand("Text.Append -suffix:Foo");
            p.AddCommand("Include -key:first");
            p.AddCommand("Text.Append -suffix:Baz");

            var result = p.Execute();

            Assert.AreEqual("FooBar1Bar2Baz", result);
        }
    }
}
