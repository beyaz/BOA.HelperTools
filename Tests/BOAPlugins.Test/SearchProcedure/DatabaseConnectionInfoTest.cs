using BOA.DatabaseAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOAPlugins.SearchProcedure.Test
{
    /// <summary>
    ///     .
    /// </summary>
    [TestClass]
    public class DatabaseConnectionInfoTest
    {
        #region Public Methods
        /// <summary>
        ///     Tests the connection strings.
        /// </summary>
        [TestMethod]
        public void Test_Connection_Strings()
        {
            Assert.IsTrue(DatabaseConnectionStrings.Connections != null);
            Assert.IsTrue(DatabaseConnectionStrings.Connections.Count > 0);

            foreach (var info in DatabaseConnectionStrings.Connections)
            {
                using (new SqlDatabase(info.ConnectionStringDev))
                {
                }
                using (new SqlDatabase(info.ConnectionStringPrep))
                {
                }
            }
        }
        #endregion
    }
}