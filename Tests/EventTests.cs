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

        [TestMethod]
        public void PipelineCreated()
        {
            Pipeline.PipelineCreated += (s, e) => { e.Pipeline.SetVariable("name", "James Bond"); }; 
            var pipeline = new Pipeline("ReadFrom name");
            var result = pipeline.Execute();
            Assert.AreEqual("James Bond", result);
        }

        [TestMethod]
        public void CommandLoading()
        {
            Pipeline.CommandLoading += (s, e) =>
            {
                if (e.FullyQualifiedCommandName.ToLower() == "text.append")
                {
                    e.Cancel = true;
                }
            };

            // We have to rerun Init, since it's run during test creation
            // This is a testing anomaly. Normally, we'd bind our events and *then* run Init()
            Pipeline.Init();

            Assert.IsTrue(!Pipeline.CommandMethods.ContainsKey("text.append"));
        }


        [TestMethod]
        public void VariableRetrieved()
        {
            var p = new Pipeline();
            p.SetVariable("foo", "bar");
            p.AddCommand("Text.Append -suffix:$foo");

            p.VariableRetrieved += (s, e) =>
            {
                if (e.Key == "foo")
                {
                    e.Value = "baz";
                }
            };

            // The variable of "foo" is initially set to "bar"
            // The event handler above should always return "baz"
            Assert.AreEqual("baz", p.Execute());
        }

        [TestMethod]
        public void VariableRetrieving()
        {
            var p = new Pipeline();
            p.AddCommand("Text.Append -suffix:$foo");

            p.VariableRetrieving += (s, e) =>
            {
                e.Pipeline.SetVariable("foo", "bar");
            };

            Assert.AreEqual("bar", p.Execute());
        }



    }
}
