using System;
using BlendInteractive.TextFilterPipeline.Core;
using BlendInteractive.TextFilterPipeline.Core.Filters;
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
            TextFilterPipeline.AddType(typeof (CustomFilters), "something");
            Assert.IsTrue(TextFilterPipeline.CommandMethods.ContainsKey("something.mymethod"));

            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("something.MyMethod");
            Assert.AreEqual("MyMethod", pipeline.Execute());
        }

        [TestMethod]
        public void OverwriteExistingFilter()
        {
            var pipeline = new TextFilterPipeline("Text.Append BAR");
            Assert.AreEqual("FOOBAR", pipeline.Execute("FOO"));

            TextFilterPipeline.AddType(typeof (OverwriteFilterTestClass));  // This should overwrite Core.Append

            Assert.AreEqual("FOOBAZ", pipeline.Execute("FOO"));

            // Now add the old filter back, or else another test fails...
            TextFilterPipeline.AddType(typeof(Core));
        }

        [TestMethod]
        public void LoadNakedMethod()
        {
            TextFilterPipeline.AddMethod(GetType().GetMethod("DoSomething"), "Deane", "DoSomething");
            
            Assert.IsTrue(TextFilterPipeline.CommandMethods.ContainsKey("deane.dosomething"));

            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("Deane.DoSomething");

            Assert.AreEqual("It worked!", pipeline.Execute());
        }

        [TestMethod]
        public void LoadMethod()
        {
            TextFilterPipeline.AddMethod(GetType().GetMethod("DoSomethingElse"), "Deane");

            Assert.IsTrue(TextFilterPipeline.CommandMethods.ContainsKey("deane.dosomethingelse"));

            var pipeline = new TextFilterPipeline();
            pipeline.AddCommand("Deane.DoSomethingElse");

            Assert.AreEqual("It worked!", pipeline.Execute());           
        }


        public static string DoSomething(string input, TextFilterCommand command)
        {
            return "It worked!";
        }

        [TextFilter("DoSomethingElse")]
        public static string DoSomethingElse(string input, TextFilterCommand command)
        {
            return "It worked!";
        }
    
    }

    [TextFilters("Text")]
    internal static class OverwriteFilterTestClass
    {
        [TextFilter("Append")]
        public static string Append(string input, TextFilterCommand command)
        {
            return String.Concat(input, "BAZ");
        }
    }


}