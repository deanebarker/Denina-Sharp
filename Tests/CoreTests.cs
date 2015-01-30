using System.IO;
using BlendInteractive.Denina.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class CoreTests
    {
        [TestMethod]
        public void WriteHelp()
        {
            //Note: This is not a test. I'm just cheating by writing out the HTML help every time I run the tests.
            File.WriteAllText("help.html", Documentor.Generate());
        }

        [TestMethod]
        public void InitVar()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("InitVar $Deane $Annie");
            pipeline.AddCommand("ReadFrom $Deane");
            pipeline.AddCommand("ReadFrom $Annie");
            pipeline.Execute();

            // No need for an assertion. If it didn't work, it would throw an exception...
        }


        [TestMethod]
        public void SetVar()
        {
            var pipeline = new Pipeline();
            pipeline.AddCommand("SetVar $Deane Awesome");
            pipeline.AddCommand("ReadFrom $Deane");
            var result = pipeline.Execute();

            Assert.AreEqual("Awesome", result);
        }
    }
}