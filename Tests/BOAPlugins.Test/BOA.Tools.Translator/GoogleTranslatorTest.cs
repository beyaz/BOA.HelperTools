using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOA.Common.Helpers;

namespace BOA.Tools.Translator
{
    [TestClass]
    public class GoogleTranslatorTest
    {
        #region Public Methods
        [TestMethod]
        public void CreatePropertyNameFromSentence()
        {
            var result = GoogleTranslator.CreatePropertyNameFromSentence("Music lesson");

            Assert.AreEqual("MusicLesson", result);
        }

        [TestMethod]
        public void TranslateEnglishToGerman()
        {
            Assert.AreEqual("Gerettet", GoogleTranslator.TranslateEnglishToGerman("Saved"));

            Assert.AreEqual("__\"Gerettet\"__", GoogleTranslator.ReplaceEmbeddedEnglishStringsWithGermanVersions("__\"Saved\"__"));



            // TranslateEnglishToGerman_TranslateExistingFile(@"d:\temp\A.txt");
        }

        void TranslateEnglishToGerman_TranslateExistingFile(string path)
        {
            var data = File.ReadAllText(path);

            var list = data.Split(Environment.NewLine.ToCharArray()).Where(x => x.IsNullOrWhiteSpace() == false).ToList();

            var convertAll = list.ConvertAll(GoogleTranslator.ReplaceEmbeddedEnglishStringsWithGermanVersions);

            var @join = string.Join(Environment.NewLine, convertAll);

            File.WriteAllText(path+".German.txt",join);
        }



        [TestMethod]
        public void TranslateTurkishToEnglish()
        {
            var result = GoogleTranslator.TranslateTurkishToEnglish("Çalışana hakkını teri kurumadan önce verin.");

            Assert.AreEqual("Give the employee the right to work before it is dried.", result);
        }

      
        #endregion
    }
}