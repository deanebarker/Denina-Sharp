using DeninaSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class FilterEvents : BaseTests
    {
        [TestMethod]
        public void OnFilterExecuting()
        {
            var p = new Pipeline("Text.Append -suffix:Baz");
            p.FilterExecuting += (s, e) =>
            {
                // This should change the input text to "Bar", no matter what was passed in
                e.Input = "Bar";
            };

            // Even thought "Foo" is passed in, the event should change the input text to "Bar" just before the only command executes
            Assert.AreEqual("BarBaz", p.Execute("Foo"));
        }

        [TestMethod]
        public void OnFilterExecutinModifyingCommand()
        {
            var p = new Pipeline("Text.Append -suffix:Baz");
            p.FilterExecuting += (s, e) =>
            {
                e.Command.CommandArgs.Clear();
                e.Command.CommandArgs.Add("suffix", "Bar");
            };

            // Even thought "Foo" is passed in, the event should change the input text to "Bar" just before the only command executes
            Assert.AreEqual("FooBar", p.Execute("Foo"));
        }

        [TestMethod]
        public void OnFilterExecuted()
        {
            var p = new Pipeline("Text.Append -suffix:Baz");
            p.FilterExecuted += (s, e) =>
            {
                // This should change the output text to "Bar", no matter what was passed in
                e.Output = "Bar";
            };

            // Even thought "Foo" is passed in, the event should change the input text to "Bar" just before the only command executes
            Assert.AreEqual("Bar", p.Execute("Foo"));
        }
    }
}
