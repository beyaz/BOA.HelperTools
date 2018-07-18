using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOAPlugins.DocumentFile
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

            var input = new Input
            {
                CSharpCode =
                    @"
A
///     <c>true</c> if uuyujyukyu
B
  /// <value>
        ///     <c>true</c> if [send cloned view model to controller]; otherwise, <c>false</c>.
        /// </value>
C
D
/// The i xxx
E
/// <exception cref=""InvalidOperationException"">@Executer must have value.</exception>
F
"
            };
            var result = handler.Handle(input);

            Assert.IsNull(result.ErrorMessage);

            Assert.AreEqual(
                @"
A
B
C
D
///     The xxx
E
F
"
                , result.CSharpCode);
        }
        #endregion




        [TestMethod]
        public void Must_Indent_When_In_Summary()
        {
            var handler = new Handler();

            var input = new Input
            {
                CSharpCode =
                    @"
A
/// <summary>
  ///  Finds the test.
/// </summary>
B
"
            };
            var result = handler.Handle(input);

            Assert.IsNull(result.ErrorMessage);

            Assert.AreEqual(
                            @"
A
/// <summary>
  ///     Finds the test.
/// </summary>
B
"
                          , result.CSharpCode);
        }
    }
}