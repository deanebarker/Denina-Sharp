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
    public class ErrorTests : BaseTests
    {
        [TestMethod]
        public void ThrowCommandException()
        {
            // We are not supplying a necessary argument
            var command = "Text.Append -foo:bar";

            var p = new Pipeline(command);
            try
            {
                // This will throw an exception
                p.Execute();
            }
            catch(DeninaException e)
            {
                Assert.AreEqual(command, e.CurrentCommandText);
                Assert.AreEqual(command.Split(" ".ToCharArray()).First().ToLower(), e.CurrentCommandName);
            }
            catch(Exception e)
            {
                Assert.Fail("We shouldn't have gotten here...");
            }
        }
    }
}
