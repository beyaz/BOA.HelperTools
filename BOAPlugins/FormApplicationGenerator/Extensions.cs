using System;
using System.Windows.Controls;
using System.Windows.Documents;
using BOA.CodeGeneration.Generators;
using BOA.Common.Helpers;

namespace BOAPlugins.FormApplicationGenerator
{
    public static class Extensions
    {
        #region Methods
        internal static string GetSnapName(this FieldInfo dataField)
        {
            return $"{dataField.ComponentName.ToString().RemoveFromStart("B").MakeLowerCaseFirstChar()}{dataField.Name}";
        }

        internal static string GetText(this RichTextBox richTextBox)
        {
            return new TextRange(richTextBox.Document.ContentStart,
                                 richTextBox.Document.ContentEnd).Text;
        }

        internal static bool HasSnapName(this FieldInfo dataField)
        {
            return dataField.ComponentName == ComponentName.BAccountComponent ||
                   dataField.ComponentName == ComponentName.BParameterComponent;
        }

        internal static void SetText(this RichTextBox richTextBox, string text)
        {
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run(text)));
        }

        internal static string ToCSharp(this DotNetTypeName name)
        {
            if (name == DotNetTypeName.Boolean)
            {
                return "bool?";
            }

            if (name == DotNetTypeName.DateTime)
            {
                return "DateTime?";
            }

            if (name == DotNetTypeName.Decimal)
            {
                return "decimal?";
            }

            if (name == DotNetTypeName.Int32)
            {
                return "int?";
            }

            if (name == DotNetTypeName.String)
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