using DeninaSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Tests
{
    [TestClass]
    public class BaseTests
    {
        public static object Lock = new object();

        [TestInitialize]
        public void TestInitialize()
        {
            Monitor.Enter(Lock);
            Pipeline.Init();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Monitor.Exit(Lock);
        }
    }
}
