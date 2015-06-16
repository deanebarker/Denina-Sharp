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
    }
}
