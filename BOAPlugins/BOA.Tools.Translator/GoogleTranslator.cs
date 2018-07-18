using System.Linq;
using BOA.Common.Helpers;

namespace BOA.Tools.Translator
{
    class GoogleTranslator
    {
        #region Public Methods
        public static string CreatePropertyNameFromSentence(string googleResult)
        {
            var result = "";

            googleResult = googleResult.Replace('(', '_')
                                       .Replace(')', '_')
                                       .Replace('-', '_')
                                       .Replace('\'', '_');

            if (googleResult.EndsWith("_"))
            {
                googleResult = googleResult.RemoveFromEnd("_");
            }

            var arr = googleResult.Split(' ');
            foreach (var value in arr)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                result += value.First().ToString().ToUpperEN() + value.Substring(1);
            }

            return result;
        }

        public static string ReplaceEmbeddedEnglishStringsWithGermanVersions(string line)
        {
            var firstIndex = line.IndexOf('"');
            var lastIndex = line.LastIndexOf('"');

            var sentence = line.Substring(firstIndex + 1, lastIndex - firstIndex - 1);

            var target = TranslateEnglishToGerman(sentence);

            target = NormalizeGermanChars(target);

            return line.Replace(sentence, target);
        }

        public static string TranslateEnglishToGerman(string sourceText)
        {
            var t = new RavSoft.GoogleTranslator.Translator
            {
                SourceLanguage = "English",
                TargetLanguage = "German",
                SourceText = sourceText
            };
            t.Translate();
            return t.Translation;
        }

        public static string TranslateTurkishToEnglish(string sourceText)
        {
            var t = new RavSoft.GoogleTranslator.Translator
            {
                SourceLanguage = "Turkish",
                TargetLanguage = "English",
                SourceText = sourceText
            };
            t.Translate();
            return t.Translation;
        }
        #endregion

        #region Methods
        static string NormalizeGermanChars(string value)
        {
            return value.Replace("Ã¼", "ü")
                        .Replace("Ã¤", "ä");
        }
        #endregion
    }
}