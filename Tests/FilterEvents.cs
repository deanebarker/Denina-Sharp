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
                // This should change the input test to "Bar", no matter what was passed in
                e.Input = "Bar";
            };

            // Even thought "Foo" is passed in, the event should change the input text to "Bar" just before the only command executes
            Assert.AreEqual("BarBaz", p.Execute("Foo"));
        }

    }
}
