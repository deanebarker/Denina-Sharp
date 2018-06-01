using DeninaSharp.Core;
using DeninaSharp.Core.Documentation;
using DeninaSharp.Core.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tests.Utility;

namespace Tests
{
    [TestClass]
    public class CustomFilterLoading : BaseTests
    {
        [TestMethod]
        public void LoadCustomFiltersFromType()
        {
            Pipeline.ReflectType(typeof (CustomFilters));
            Assert.IsTrue(Pipeline.CommandMethods.ContainsKey("custom.mymethod"));

            var pipeline = new Pipeline();
            pipeline.AddCommand("custom.MyMethod");
            Assert.AreEqual("MyMethod", pipeline.Execute());
        }

        [TestMethod]
        public void LoadCustomFiltersFromTypeWithCategoryName()
        {
            Pipeline.ReflectType(typeof (CustomFilters), "something");
            Assert.IsTrue(Pipeline.CommandMethods.ContainsKey("something.mymethod"));

            var pipeline = new Pipeline();
            pipeline.AddCommand("something.MyMethod");
            Assert.AreEqual("MyMethod", pipeline.Execute());
        }

        [TestMethod]
        public void OverwriteExistingFilter()
        {
            var pipeline = new Pipeline("Text.Append -suffix:BAR");
            Assert.AreEqual("FOOBAR", pipeline.Execute("FOO"));

            Pipeline.ReflectType(typeof (OverwriteFilterTestClass));  // This should overwrite Core.Append

            Assert.AreEqual("FOOBAZ", pipeline.Execute("FOO"));

            // Now add the old filter back, or else another test fails...
            Pipeline.ReflectType(typeof(Core));
        }

        [TestMethod]
        public void LoadNakedMethod()
        {
            Pipeline.AddMethod(GetType().GetMethod("DoSomething"), "Deane", "DoSomething");
            
            Assert.IsTrue(Pipeline.CommandMethods.ContainsKey("deane.dosomething"));

            var pipeline = new Pipeline();
            pipeline.AddCommand("Deane.DoSomething");

            Assert.AreEqual("It worked!", pipeline.Execute());
        }

        [TestMethod]
        public void LoadMethod()
        {
            Pipeline.ReflectMethod(GetType().GetMethod("DoSomethingElse"), "Deane");

            Assert.IsTrue(Pipeline.CommandMethods.ContainsKey("deane.dosomethingelse"));

            var pipeline = new Pipeline();
            pipeline.AddCommand("Deane.DoSomethingElse");

            Assert.AreEqual("It worked!", pipeline.Execute());           
        }

        [TestMethod]
        public void TryToLoadDependency()
        {
            var pipeline = new Pipeline();

            Assert.IsTrue(Pipeline.CommandMethods.ContainsKey("html.extract"));
        }

        [TestMethod]
        public void TryToLoadBrokenDependency()
        {
            Pipeline.ReflectMethod(GetType().GetMethod("NeedsMissingDependency"), "Test");

            var pipeline = new Pipeline();
            

            Assert.IsFalse(Pipeline.CommandMethods.ContainsKey("test.needsmissingdependency"));

            // Check that the correct exception text is thrown
            try
            {
                pipeline.AddCommand("test.needsmissingdependency");
                pipeline.Execute("");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("dependency"));
                return;
            }

            Assert.Fail("We shouldn't get here...");
        }

        [TestMethod]
        public void LoadUnderMultipleCommandNames()
        {
            Pipeline.ReflectType(typeof(MultiNameTest));

            Assert.IsTrue(Pipeline.CommandMethods["multinametest.foo"] != null);
            Assert.IsTrue(Pipeline.CommandMethods["multinametest.bar"] != null);
        }

        [TestMethod]
        public void AddAnonymousFunction()
        {
            var category = "AnonymousFunctionTest";
            var name = "myFilter";
            var fqName = string.Concat(category.ToLower(), ".", name.ToLower());

            Func<string, PipelineCommand, ExecutionLog, string> myFilter = (input, command, log) =>
            {
                return "It worked!";
            };

            Pipeline.AddMethod(myFilter.Method, category, name);

            Assert.IsTrue(Pipeline.CommandMethods.ContainsKey(fqName));

            var pipeline = new Pipeline(fqName);
            Assert.AreEqual("It worked!", pipeline.Execute());
        }


        public static string DoSomething(string input, PipelineCommand command, ExecutionLog log)
        {
            return "It worked!";
        }

        [Filter("DoSomethingElse")]
        public static string DoSomethingElse(string input, PipelineCommand command, ExecutionLog log)
        {
            return "It worked!";
        }

        [Filter("FilterWithFakeDependency")]
        [Requires("MissingType, MissingAssembly", "")]
        public static string NeedsMissingDependency(string input, PipelineCommand command, ExecutionLog log)
        {
            return "It worked!";
        }

    }

    [Filters("Text")]
    internal static class OverwriteFilterTestClass
    {
        [Filter("Append")]
        public static string Append(string input, PipelineCommand command, ExecutionLog log)
        {
            return String.Concat(input, "BAZ");
        }
    }

    [Filters("MultiNameTest")]
    internal static class MultiNameTest
    {
        [Filter("Foo")]
        [Filter("Bar")]
        public static string MultiNameTestFilter(string input, PipelineCommand command, ExecutionLog log)
        {
            return input;
        }
    }


}