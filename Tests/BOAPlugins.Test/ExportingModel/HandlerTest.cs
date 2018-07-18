using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOAPlugins.ExportingModel
{
    [TestClass]
    public class HandlerTest
    {
        #region Public Methods
        [TestMethod]
        public void Handle()
        {
            var path   = @"D:\work\BOA.BusinessModules\Dev\BOA.CardGeneral.DebitCard\BOA.CardGeneral.DebitCard.sln";
            var result = Handler.Handle(path);

            Assert.IsNull(result.ErrorMessage);
        }
        #endregion
    }
}