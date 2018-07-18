using BOAPlugins.ViewClassDependency;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOAPlugins.Test.ViewClassDependency
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
        public void Test_Call_Graph()
        {
            var result = new Handler().Handle(new Data
            {
                OutputFileFullPath = @"D:\Users\beyaztas\Documents\ClassDependencyView.dgml",
                SelectedText = "ProvisionTechnicalEngine",
                AssemblySearchDirectoryPath = @"D:\work\BOA.Kernel\Dev\BOA.Kernel.CardGeneral\DebitCard\BOA.Engine.DebitCard\bin\Debug\"
            });

            Assert.IsNull(result.ErrorMessage);
        }

        /// <summary>
        ///     Tries to find definition automaticly.
        /// </summary>
        [TestMethod]
        public void TryToFindDefinitionAutomaticly()
        {
            // ARRANGE
            var data = new Data
            {
                SelectedText = "LogFileSerializer",
                AssemblySearchDirectoryPath = @"D:\Work\BOA.Kernel\Dev\BOA.Kernel.CardGeneral\DebitCard\BOA.Types.Kernel.DebitCard\bin\Debug\"
            };
            var api = new Handler();

            // ACT
            var definition = api.TryToFindDefinitionAutomaticly(data);

            // ASSERT
            Assert.IsNotNull(definition);

            // D:\work\BOA.BusinessModules\Dev\BOA.CardGeneral.DebitCard\UI\BOA.UI.CardGeneral.DebitCard.CardPrinting\bin\Debug
        }
        #endregion
    }
}