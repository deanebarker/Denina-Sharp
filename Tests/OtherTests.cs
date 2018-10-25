using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeninaSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeninaSharp.Core.Filters;

namespace Tests
{
    [TestClass]
    public class OtherTests : BaseTests
    {
        [TestMethod]
        public void DebugData()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.Prepend -prefix:Deane");
            pipeline.AddCommand("Text.Append -suffix:Barker");
            pipeline.Execute();

            Assert.AreEqual(pipeline.LogEntries.Count, 2);
            Assert.AreEqual(pipeline.LogEntries.First().InputValue.Length, 0);
            Assert.AreEqual(pipeline.LogEntries.First().OutputValue.Length, 5);
            Assert.AreEqual(pipeline.LogEntries.Last().InputValue.Length, 5);
            Assert.AreEqual(pipeline.LogEntries.Last().OutputValue.Length, 11);
        }

        [TestMethod]
        public void RemoveFilter()
        {
            Func<string, PipelineCommand, ExecutionLog, string> myFilter = (input, command, log) =>
            {
                return "It worked!";
            };
            Pipeline.ReflectMethod(myFilter.Method, "Foo", "Bar");

            Assert.IsTrue(Pipeline.CommandMethods.ContainsKey("foo.bar"));
            Pipeline.RemoveCommand("Foo.Bar", "Never use this!!!");
            Assert.IsFalse(Pipeline.CommandMethods.ContainsKey("foo.bar"));

            var pipeline = new Pipeline();
            pipeline.AddCommand("Foo.Bar");

            try
            {
                pipeline.Execute("baz");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("!!!"));
            }
        }


        [TestMethod]
        public void RemoveFilterCategory()
        {
            Assert.IsTrue(Pipeline.CommandMethods.ContainsKey("text.append"));
            Pipeline.RemoveCommandCategory("text");
            Assert.IsFalse(Pipeline.CommandMethods.ContainsKey("text.append"));
        }

        [TestMethod]
        public void GetLogicHashCode()
        {
            // These two commands are the same, just with arguments in different order. Their LogicHashCode should be the same.
            var p1 = new PipelineCommand() { FullyQualifiedCommandName = "James.Bond", CommandArgs = new Dictionary<object, string>() { { "foo", "bar" }, { "bar", "baz" } } };
            var p2 = new PipelineCommand() { FullyQualifiedCommandName = "James.Bond", CommandArgs = new Dictionary<object, string>() { { "bar", "baz" }, { "foo", "bar" } } };

            Assert.AreEqual(p1.GetLogicHashCode(), p2.GetLogicHashCode());

            // Now, let's make a small change
            p2.CommandArgs["foo"] = "foo";

            // Their LogicHashCodes should differ now
            Assert.AreNotEqual(p1.GetLogicHashCode(), p2.GetLogicHashCode());

            // We should also be able to compute a has with no arguments
            var p3 = new PipelineCommand() { FullyQualifiedCommandName = "James.Bond" };
            try
            {
                var result = p3.GetLogicHashCode();
            }
            catch(Exception e)
            {
                Assert.Fail("Exception when computing LogicHashCode of command with no arguments.");
            }
        }
    }
}
