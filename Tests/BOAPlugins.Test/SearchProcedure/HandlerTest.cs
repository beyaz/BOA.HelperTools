using BOAPlugins.SearchProcedure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOAPlugins.Test.SearchProcedure
{
    /// <summary>
    ///     .
    /// </summary>
    [TestClass]
    public class HandlerTest
    {
        #region Public Methods
        /// <summary>
        ///     Finds the test.
        /// </summary>
        [TestMethod]
        public void FindProcedureTest()
        {
            var procedureName = "CRD.MoneySendTransactionDetail";
            var handler = new Handler(new Input {ProcedureName = procedureName});
            handler.Handle();

            Assert.IsNull(handler.Result.ErrorMessage);
            Assert.IsNotNull(handler.Result.ProcedureContainerDatabaseConnectionInfo);
        }
        #endregion
    }
}