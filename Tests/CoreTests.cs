using DeninaSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class CoreTests : BaseTests
    {
        [TestMethod]
        public void InitVar()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("InitVar -var:Deane -var:Annie");
            pipeline.AddCommand("ReadFrom Deane");
            pipeline.AddCommand("ReadFrom Annie");
            pipeline.Execute();

            // No need for an assertion. If it didn't work, it would throw an exception...
        }
        
        [TestMethod]
        public void SetVar()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("SetVar -var:Deane -value:Awesome");
            pipeline.AddCommand("ReadFrom Deane");
            var result = pipeline.Execute();

            Assert.AreEqual("Awesome", result);
        }
    }
}