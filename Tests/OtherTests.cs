using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeninaSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class OtherTests
    {
        [TestInitialize]
        public void Init()
        {
            Pipeline.Init();
        }

        [TestMethod]
        public void DebugData()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.Prepend -prefix:Deane");
            pipeline.AddCommand("Text.Append -suffix:Barker");
            pipeline.Execute();

            Assert.AreEqual(pipeline.DebugData.Count, 2);
            Assert.AreEqual(pipeline.DebugData.First().InputValue.Length, 0);
            Assert.AreEqual(pipeline.DebugData.First().OutputValue.Length, 5);
            Assert.AreEqual(pipeline.DebugData.Last().InputValue.Length, 5);
            Assert.AreEqual(pipeline.DebugData.Last().OutputValue.Length, 11);
        }

        [TestMethod]
        public void UnloadCommand()
        {
            Assert.IsTrue(Pipeline.CommandMethods.ContainsKey("text.append"));
            Pipeline.RemoveCommand("text.append", "Never append text to anything!!!");
            Assert.IsFalse(Pipeline.CommandMethods.ContainsKey("text.append"));
            
            var pipeline = new Pipeline();
            pipeline.AddCommand("Text.Append -prefix:foo");

            try
            {
                pipeline.Execute("bar");
            }
            catch(Exception e)
            {
                Assert.IsTrue(e.Message.Contains("!!!"));
            }

        }


        [TestMethod]
        public void UnloadCategory()
        {
            Pipeline.Init();
            Assert.IsTrue(Pipeline.CommandMethods.ContainsKey("text.append"));
            Pipeline.RemoveCategory("text");
            Assert.IsFalse(Pipeline.CommandMethods.ContainsKey("text.append"));
        }
    }
}
