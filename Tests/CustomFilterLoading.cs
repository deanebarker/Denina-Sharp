using System;
using BlendInteractive.TextFilterPipeline.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Utility;

namespace Tests
{
    [TestClass]
    public class CustomFilterLoading
    {
        [TestMethod]
        public void LoadCustomFiltersFromType()
        {
            TextFilterPipeline.AddType(typeof (CustomFilters));
            Assert.IsTrue(TextFilterPipeline.CommandMethods.ContainsKey("custom.mymethod"));

            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("custom.MyMethod");
            Assert.AreEqual("MyMethod", pipeline.Execute());
        }

        [TestMethod]
        public void LoadCustomFiltersFromTypeWithCategoryName()
        {
            TextFilterPipeline.AddType("something", typeof (CustomFilters));
            Assert.IsTrue(TextFilterPipeline.CommandMethods.ContainsKey("something.mymethod"));

            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("something.MyMethod");
            Assert.AreEqual("MyMethod", pipeline.Execute());
        }

        [TestMethod]
        public void OverwriteExistingFilter()
        {
            var pipeline = new TextFilterPipeline("Append BAR");
            Assert.AreEqual("FOOBAR", pipeline.Execute("FOO"));

            TextFilterPipeline.AddType(typeof (OverwriteFilterTestClass));  // This should overwrite Core.Append

            Assert.AreEqual("FOOBAZ", pipeline.Execute("FOO"));
        }
    }

    [TextFilters("Core")]
    internal static class OverwriteFilterTestClass
    {
        [TextFilter("Append")]
        public static string Append(string input, TextFilterCommand command)
        {
            return String.Concat(input, "BAZ");
        }
    }
}