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
        public void ReflectFiltersFromTypeWithFiltersAttributeAndCategoryName()
        {
            Pipeline.ReflectType(typeof(TypeWithFiltersAttributeAndCategoryName));
            Assert.IsTrue(Pipeline.CommandMethods.ContainsKey("filtersattributename.myfilter"));
        }

        [TestMethod]
        public void ReflectFiltersFromTypeWithFiltersAttributeButNoCategoryName()
        {
            Pipeline.ReflectType(typeof(TypeWithFiltersAttributeButNoCategoryName));
            Assert.IsTrue(Pipeline.CommandMethods.ContainsKey("typewithfiltersattributebutnocategoryname.myfilter"));
        }

        [TestMethod]
        public void ReflectFiltersFromTypeWithNoFiltersAttribute()
        {
            Pipeline.ReflectType(typeof(TypeWithNoFiltersAttribute));
            Assert.IsTrue(Pipeline.CommandMethods.ContainsKey("typewithnofiltersattribute.myfilter"));
        }

        [TestMethod]
        public void OverwriteExistingFilter()
        {
            var pipeline = new Pipeline("Text.Append -suffix:BAR");
            Assert.AreEqual("FOOBAR", pipeline.Execute("FOO"));

            Pipeline.ReflectType(typeof(OverwriteFilterTestClass));  // This should overwrite Core.Append

            Assert.AreEqual("FOOBAZ", pipeline.Execute("FOO"));

            // Now add the old filter back, or else another test fails...
            Pipeline.ReflectType(typeof(Core));
        }

        [TestMethod]
        public void AddMethodWithNoFilterAttribute()
        {
            Pipeline.ReflectMethod(GetType().GetMethod("FilterMethodWithNoFilterAttribute"), "MyCategory", "MyFilter");
            Assert.IsTrue(Pipeline.CommandMethods.ContainsKey("mycategory.myfilter"));
        }

        [TestMethod]
        public void ReflectMethodWithFilterAttribute()
        {
            Pipeline.ReflectMethod(GetType().GetMethod("FilterMethodWithFilterAttribute"));
            Assert.IsTrue(Pipeline.CommandMethods.ContainsKey("customfilterloading.myfilter"));
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

            Pipeline.ReflectMethod(myFilter.Method, category, name);

            Assert.IsTrue(Pipeline.CommandMethods.ContainsKey(fqName));

            var pipeline = new Pipeline(fqName);
            Assert.AreEqual("It worked!", pipeline.Execute());
        }


        public static string FilterMethodWithNoFilterAttribute(string input, PipelineCommand command, ExecutionLog log)
        {
            return "It worked!";
        }

        [Filter("MyFilter")]
        public static string FilterMethodWithFilterAttribute(string input, PipelineCommand command, ExecutionLog log)
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

    internal static class TypeWithNoFiltersAttribute
    {
        [Filter]
        public static string MyFilter(string input, PipelineCommand command, ExecutionLog log)
        {
            return input;
        }
    }

    [Filters]
    internal static class TypeWithFiltersAttributeButNoCategoryName
    {
        [Filter]
        public static string MyFilter(string input, PipelineCommand command, ExecutionLog log)
        {
            return input;
        }
    }

    [Filters("FiltersAttributeName")]
    internal static class TypeWithFiltersAttributeAndCategoryName
    {
        [Filter]
        public static string MyFilter(string input, PipelineCommand command, ExecutionLog log)
        {
            return input;
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