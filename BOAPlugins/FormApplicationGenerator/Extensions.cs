using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using BOA.CodeGeneration.Generators;
using BOA.Common.Helpers;

namespace BOAPlugins.FormApplicationGenerator
{
    public static class Extensions
    {
        #region Public Methods
        public static void AutoGenerateCodesAndExportFiles(this Model model)
        {
            new FileExporter(model).ExportFiles();
        }
        public static IReadOnlyCollection<BField> GetAllFields(this IReadOnlyCollection<BCard> cards)
        {
            var allFields = new List<BField>();

            foreach (var card in cards)
            {
                allFields.AddRange(card.Fields);
            }

            return allFields;
        }
        #endregion

        #region Methods
        internal static string GetSnapName(this BField dataBField)
        {
            return $"{dataBField.ComponentType.ToString().RemoveFromStart("B").MakeLowerCaseFirstChar()}{dataBField.Name}";
        }

        internal static string GetText(this RichTextBox richTextBox)
        {
            return new TextRange(richTextBox.Document.ContentStart,
                                 richTextBox.Document.ContentEnd).Text;
        }

        internal static bool HasSnapName(this BField dataBField)
        {
            return dataBField.ComponentType == ComponentType.BAccountComponent ||
                   dataBField.ComponentType == ComponentType.BParameterComponent;
        }

        internal static void SetText(this RichTextBox richTextBox, string text)
        {
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run(text)));
        }

        internal static string ToCSharp(this DotNetType name)
        {
            if (name == DotNetType.Boolean)
            {
                return "bool?";
            }

            if (name == DotNetType.DateTime)
            {
                return "DateTime?";
            }

            if (name == DotNetType.Decimal)
            {
                return "decimal?";
            }

            if (name == DotNetType.Int32)
            {
                return "int?";
            }

            if (name == DotNetType.String)
            {
                return "string";
            }

            throw new ArgumentException(name.ToString());
        }

        static string MakeLowerCaseFirstChar(this string value)
        {
            if (value.IsNullOrEmpty())
            {
                return value;
            }

            return ContractBodyGenerator.GetPropertyFieldName("", value);
        }
        #endregion
    }
}