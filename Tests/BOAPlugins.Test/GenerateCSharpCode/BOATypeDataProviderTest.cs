using BOAPlugins.TypeSearchView;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOAPlugins.Test.GenerateCSharpCode
{
    [TestClass]
    public class BOATypeDataProviderTest
    {
        #region Public Methods
        [TestMethod]
        public void GetAllTypes()
        {
            var types = new BOATypeDataProvider().GetAllTypes();

            Assert.IsTrue(types.Count > 0);

            types = new BOATypeDataProvider().GetAllTypes(@"d:\boa\client\bin\");

            Assert.IsTrue(types.Count > 0);
        }
        #endregion
    }
}