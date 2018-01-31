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
    public class EventTests : BaseTests
    {
        [TestMethod]
        public void PipelineComplete()
        {
            Pipeline.PipelineComplete += (s, e) => { e.Value = "foo"; }; // This should just totally replace the value to prove the event fired
            var pipeline = new Pipeline();
            var result = pipeline.Execute("bar");  // Without the event handler, this should just pass "bar" through

            Assert.AreEqual("foo", result);
        }
    }
}
