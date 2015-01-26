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
    }
}