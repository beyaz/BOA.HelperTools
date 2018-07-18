using System;
using System.IO;
using BOAPlugins.SearchProcedure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOAPlugins.Test.SearchProcedure
{
    [TestClass]
    public class SpaceCaseInsensitiveComparatorTest
    {
        #region Public Methods
        [TestMethod]
        public void CanBeIgnoreSpecificLines()
        {
            var data1 = File.ReadAllText(@"SearchProcedure\SpaceCaseInsensitiveComparatorTest.Data1.txt");
            var data2 = File.ReadAllText(@"SearchProcedure\SpaceCaseInsensitiveComparatorTest.Data2.txt");

            var comparor = new SpaceCaseInsensitiveComparator();
            comparor.IgnoreLines(line => line.StartsWith("CertificateInformation | K", StringComparison.Ordinal));
            Assert.IsTrue(comparor.Compare(data1, data2));
        }
        #endregion
    }
}