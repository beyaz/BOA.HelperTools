using System;
using System.Collections.Generic;
using System.Text;
using WhiteStone.Mvc;

namespace BOA.Tools.Translator.UI.TranslateHelper
{
    class Controller : ControllerBase<Model>
    {
        #region Static Fields
        static readonly Dictionary<string, string> TR_EN_Cache = new Dictionary<string, string>();
        #endregion

        #region Public Methods
        public void InputChanged()
        {
            var propertyDefinitions = new StringBuilder();
            var propertyAssignmentsForEN = new StringBuilder();

            foreach (var trValue in Model.Input.Split(Environment.NewLine.ToCharArray()))
            {
                if (string.IsNullOrWhiteSpace(trValue))
                {
                    continue;
                }

                var turkishText = trValue.Trim();

                var english = TranslateTurkishToEnglish(turkishText);

                var propertyName = GoogleTranslator.CreatePropertyNameFromSentence(english);

                propertyDefinitions.AppendLine($"public string {propertyName} " + "{get;set;}");

                propertyAssignmentsForEN.AppendLine(propertyName + "=" + '"' + english + '"' + ';');
            }

            Model.PropertyDefinition = propertyDefinitions.ToString();
            Model.PropertyAssignmentForEN = propertyAssignmentsForEN.ToString();
        }
        #endregion

        #region Methods
        static string TranslateTurkishToEnglish(string turkishText)
        {
            string english = null;

            if (TR_EN_Cache.TryGetValue(turkishText, out english))
            {
                return english;
            }

            english = GoogleTranslator.TranslateTurkishToEnglish(turkishText);

            TR_EN_Cache[turkishText] = english;

            return english;
        }
        #endregion
    }
}