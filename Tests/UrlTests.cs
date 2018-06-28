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
    public class UrlTests : BaseTests
    {
        [TestMethod]
        public void AddQuerystringArg()
        {
            var url = "http://example.com/";
            var p = new Pipeline();
            p.AddCommand("Url.AddQuerystringArg -key:a -value:b");
            var result = p.Execute(url);

            Assert.AreEqual("http://example.com/?a=b", result);
        }

        [TestMethod]
        public void ChangeQuerystringArg()
        {
            var url = "http://example.com/?a=b";
            var p = new Pipeline();
            p.AddCommand("Url.AddQuerystringArg -key:a -value:c");
            var result = p.Execute(url);

            Assert.AreEqual("http://example.com/?a=c", result);
        }
    }
}
