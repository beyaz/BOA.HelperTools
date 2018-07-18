using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOA.CodeGeneration.Util
{
    [TestClass]
    public class TFSAccessForBOATest
    {
        #region Properties
        static TFSAccessForBOA Api => new TFSAccessForBOA();
        #endregion

        #region Public Methods
        [TestMethod]
        public void CheckinFile()
        {
            // ARRANGE
            var path = @"D:\work\BOA.Retired\Dev\BOA.Kernel.DataAccess\BOA.CodeGeneration\Common\SqlReaderMethods.cs";

            Api.CheckoutFile(path);

            // ACT  + ASSERT
            Api.CheckinFile(path, "INC292087# auto check in.");
        }

        [TestMethod]
        public void CheckinSolution()
        {
            var input = new CheckinSolutionInput
            {
                Comment = "INC292087# -",
                SolutionFilePath = @"D:\work\BOA.Retired\Dev\BOA.Kernel.DataAccess\BOA.Kernel.DataAccess.sln"
            };

            Api.CheckinSolution(input);
        }

        [TestMethod]
        public void ChecoutFile()
        {
            // ARRANGE
            var path = @"D:\work\BOA.Retired\Dev\BOA.Kernel.DataAccess\BOA.CodeGeneration\Common\SqlReaderMethods.cs";
            // path = @"D:\workde\BOA.BusinessModules\Dev\BOA.Card.CreditCardOperation\BOA.Business.Card.CreditCardOperation\BOA.Business.Kernel.CreditCard\CreditCardInstallmentTransaction.designer.cs";

            // ACT
            var isSuccess = Api.CheckoutFile(path);

            // ASSERT
            Assert.IsTrue(isSuccess);
        }
        #endregion
    }
}