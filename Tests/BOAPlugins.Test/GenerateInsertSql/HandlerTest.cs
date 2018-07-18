using BOAPlugins.GenerateInsertSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOAPlugins.Test.GenerateInsertSql
{
    /// <summary>
    ///     Defines the handler test.
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
            var handler = new Handler();

            var result = handler.Handle(new Input {TableName = "CRD.DebitCardApplication"});

            Assert.IsNull(result.ErrorMessage);
        }
        #endregion
    }
}