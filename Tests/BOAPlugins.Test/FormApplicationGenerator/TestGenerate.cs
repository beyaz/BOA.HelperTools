using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOAPlugins.FormApplicationGenerator
{
    [TestClass]
    public class TestGenerate
    {
        #region Public Methods
        [TestMethod]
        public void T1()
        {
            new VisaFee().AutoGenerateCodesAndExportFiles();
        }
        #endregion
    }
}