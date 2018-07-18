using BOAPlugins.GenerateEntityContract;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOAPlugins.Test.GenerateEntityContract
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
        public void Test_Table_Names()
        {
            var result = new Handler().Handle(new Input {SelectedText = "DBT.ZoneKey"});

            Assert.IsNull(result.ErrorMessage);
        }
        #endregion
    }
}