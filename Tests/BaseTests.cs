using DeninaSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Tests
{
    [TestClass]
    public class BaseTests
    {
        public static object Lock = new object();
        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            TestContext.WriteLine("Start");
            Monitor.Enter(Lock);
            Pipeline.Reset();
            Pipeline.Init();
            TestContext.WriteLine($"Init complete. {Pipeline.CommandMethods.Count} command(s) loaded.");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestContext.WriteLine("End");
            Monitor.Exit(Lock);
        }
    }
}
