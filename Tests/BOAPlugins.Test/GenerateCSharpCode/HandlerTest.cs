using System.IO;
using BOA.CodeGeneration.Services;
using BOAPlugins.GenerateCSharpCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WhiteStone.Helpers;

namespace BOAPlugins.Test.GenerateCSharpCode
{
    /// <summary>
    ///     .
    /// </summary>
    [TestClass]
    public class HandlerTest
    {
        #region Public Methods
        [TestMethod]
        public void DeleteTest()
        {
            // ARRANGE
            var input = new Input
            {
                ProcedureName = "[DBT].[del_CardAccountByCardNumber]"
            };
            var expectedOutput = GetFileContent("DBT.del_CardAccountByCardNumber.txt");

            var handler = new Handler(input);

            // ACT
            handler.Handle();

            // ASSERT
            Assert.IsTrue(Compare(expectedOutput, handler.Result.GeneratedCsCode));
        }

        /// <summary>
        ///     Gets the method name from procedure.
        /// </summary>
        [TestMethod]
        public void GetMethodNameFromProcedure()
        {
            Assert.AreEqual("SelectZoneKeyByZoneCode", Handler.GetMethodNameFromProcedure("DBT.sel_ZoneKeyByZoneCode"));
        }

        /// <summary>
        ///     Tests for select procedures.
        /// </summary>
        [TestMethod]
        public void GuidTypesMustBeHandle()
        {
            // ARRANGE
            var input = new Input
            {
                ProcedureName = "DBT.sel_DebitTransactionTrace"
            };
            var expectedOutput = GetFileContent("DBT.sel_DebitTransactionTrace.txt");

            var handler = new Mock<Handler>(input)
                {
                    CallBase = true
                }.GetReturnTypeNameFromUser_Returns("BOA.Types.CardGeneral.AtmTcpListener.CommunicationInfoSearchContract")
                 .Object;

            // ACT
            handler.Handle();

            // ASSERT
            Assert.IsTrue(Compare(expectedOutput, handler.Result.GeneratedCsCode));
        }

        /// <summary>
        ///     Handles this instance.
        /// </summary>
        [TestMethod]
        public void Handle()
        {
            // ARRANGE
            var input = new Input
            {
                ProcedureName = "[DBT].[sel_KMHLimitByDebitCard]"
            };

            const string userEnteredTypeName = "BOA.Types.Kernel.DebitCard.DebitCardKMHInfoContract";
            var expectedOutput = GetFileContent("DBT.sel_KMHLimitByDebitCard.cs.txt");

            var handler = new Mock<Handler>(input)
                {
                    CallBase = true
                }.GetReturnTypeNameFromUser_Returns(userEnteredTypeName)
                 .Object;

            // ACT
            handler.Handle();

            // ASSERT
            Assert.IsTrue(Compare(expectedOutput, handler.Result.GeneratedCsCode));
        }

        /// <summary>
        ///     Inserts the procedure with comment.
        /// </summary>
        [TestMethod]
        public void Insert_Procedure_With_Comment()
        {
            // ARRANGE
            var input = new Input
            {
                ProcedureName = "DBT.ins_Bkm3DSecureNotifyRecord"
            };
            var expectedOutput = GetFileContent("InsertBkm3DSecureNotifyRecord.txt");

            var handler = new Mock<Handler>(input)
                {
                    CallBase = true
                }.GetReturnTypeNameFromUser_Returns("BOA.Types.Kernel.DebitCard.Bkm3DSecureContract")
                 .Object;

            // ACT
            handler.Handle();

            // ASSERT
            Assert.IsTrue(Compare(expectedOutput, handler.Result.GeneratedCsCode));
        }

        /// <summary>
        ///     Called when [column return must be return list].
        /// </summary>
        [TestMethod]
        public void OneColumnReturn_Must_Be_Return_List()
        {
            // ARRANGE
            var input = new Input
            {
                ProcedureName = "DBT.sel_CardInfoCardNumberListByAccountNumber"
            };
            var expectedOutput = GetFileContent("SelectCardInfoCardNumberListByAccountNumber.cs.txt");

            var handler = new Mock<Handler>(input)
                {
                    CallBase = true
                }.GetReturnTypeNameFromUser_Returns(null)
                 .Object;

            // ACT
            handler.Handle();

            // ASSERT
            Assert.IsTrue(Compare(expectedOutput, handler.Result.GeneratedCsCode));
        }

        /// <summary>
        ///     Tests for execute scalar.
        /// </summary>
        [TestMethod]
        public void Test_for_ExecuteScalar()
        {
            // ARRANGE
            var input = new Input
            {
                ProcedureName = "DBT.sel_DebitCardApplicationCustomerDefaultAddressId"
            };
            var expectedOutput = GetFileContent("SelectDebitCardApplicationCustomerDefaultAddressId.cs.txt");

            var handler = new Mock<Handler>(input)
                {
                    CallBase = true
                }.GetReturnTypeNameFromUser_Returns(null)
                 .Object;

            // ACT
            handler.Handle();

            // ASSERT
            Assert.IsTrue(Compare(expectedOutput, handler.Result.GeneratedCsCode));
        }

        /// <summary>
        ///     Tests for insert.
        /// </summary>
        [TestMethod]
        public void Test_for_Insert()
        {
            // ARRANGE
            var input = new Input {ProcedureName = "DBT.ins_CustomerMemo"};
            const string userEnteredTypeName = "BOA.Types.Kernel.DebitCard.IReadOnlyCustomerMemoContract";
            var expectedOutput = GetFileContent("InsertCustomerMemo.cs.txt");

            var handler = new Mock<Handler>(input)
                {
                    CallBase = true
                }.GetReturnTypeNameFromUser_Returns(userEnteredTypeName)
                 .Object;

            // ACT
            handler.Handle();

            // ASSERT
            Assert.IsTrue(Compare(expectedOutput, handler.Result.GeneratedCsCode));
        }

        /// <summary>
        ///     Tests for nullable types.
        /// </summary>
        [TestMethod]
        public void Test_for_nullable_Types()
        {
            // ARRANGE
            var input = new Input {ProcedureName = "DBT.sel_ZoneKeyByZoneCode"};
            const string userEnteredTypeName = "BOA.Types.Kernel.DebitCard.ZoneKeyContract";
            var expectedOutput = GetFileContent("SelectZoneKeyByZoneCode.cs.txt");

            var handler = new Mock<Handler>(input)
                {
                    CallBase = true
                }.GetReturnTypeNameFromUser_Returns(userEnteredTypeName)
                 .Object;

            // ACT
            handler.Handle();

            // ASSERT
            Assert.IsTrue(Compare(expectedOutput, handler.Result.GeneratedCsCode));
        }

        /// <summary>
        ///     Tests for update procedures with contract input.
        /// </summary>
        [TestMethod]
        public void Test_for_Update_Procedures_With_Contract_Input()
        {
            // ARRANGE
            var input = new Input {ProcedureName = "DBT.upd_DebitCardApplicationKMHInformations"};
            const string userEnteredTypeName = "BOA.Types.Kernel.DebitCard.DebitCardApplicationContract";
            var expectedOutput = GetFileContent("UpdateDebitCardApplicationKMHInformations.cs.txt");

            var handler = new Mock<Handler>(input)
                {
                    CallBase = true
                }.GetReturnTypeNameFromUser_Returns(userEnteredTypeName)
                 .Object;

            // ACT
            handler.Handle();

            // ASSERT
            Assert.IsTrue(Compare(expectedOutput, handler.Result.GeneratedCsCode));
        }

        /// <summary>
        ///     Tests for update procedures with parameter input.
        /// </summary>
        [TestMethod]
        public void Test_for_Update_Procedures_With_parameter_Input()
        {
            // ARRANGE
            var input = new Input {ProcedureName = "DBT.upd_CardInUsePin"};
            var expectedOutput = GetFileContent("UpdateCardInUsePin.cs.txt");

            var handler = new Mock<Handler>(input)
                {
                    CallBase = true
                }.GetReturnTypeNameFromUser_Returns(null)
                 .Object;

            // ACT
            handler.Handle();

            // ASSERT
            Assert.IsTrue(Compare(expectedOutput, handler.Result.GeneratedCsCode));
        }

        /// <summary>
        ///     Tests for select procedures.
        /// </summary>
        [TestMethod]
        public void TestForSelectProcedures()
        {
            // ARRANGE
            var input = new Input
            {
                ProcedureName = "DBT.sel_Bkm3DSecureNotifyById"
            };
            var expectedOutput = GetFileContent("SelectBkm3DSecureNotifyById.txt");

            var handler = new Mock<Handler>(input)
                {
                    CallBase = true
                }.GetReturnTypeNameFromUser_Returns("BOA.Types.Kernel.DebitCard.Bkm3DSecureContract")
                 .Object;

            // ACT
            handler.Handle();

            // ASSERT
            Assert.IsTrue(Compare(expectedOutput, handler.Result.GeneratedCsCode));
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Compares the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        static bool Compare(string left, string right)
        {
            return SpaceCaseInsensitiveComparator.Compare(left, right, GlobalizationUtility.EnglishCulture);
        }

        /// <summary>
        ///     Gets the content of the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        static string GetFileContent(string path)
        {
            path = nameof(GenerateCSharpCode) + Path.DirectorySeparatorChar + path;
            return File.ReadAllText(path);
        }
        #endregion
    }

    static class Extensions
    {
        #region Methods
        internal static Mock<Handler> GetReturnTypeNameFromUser_Returns(this Mock<Handler> mock, string contractName)
        {
            mock.Setup(m => m.GetReturnTypeNameFromUser()).Returns(() => contractName);
            return mock;
        }
        #endregion
    }
}