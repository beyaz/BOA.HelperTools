using System.Windows.Controls;
using System.Windows.Documents;

namespace BOAPlugins.FormApplicationGenerator
{
    public static class Ext
    {
        #region Public Methods
        public static string GetText(this RichTextBox richTextBox)
        {
            return new TextRange(richTextBox.Document.ContentStart,
                                 richTextBox.Document.ContentEnd).Text;
        }

        public static void SetText(this RichTextBox richTextBox, string text)
        {
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run(text)));
        }
        #endregion
    }
}