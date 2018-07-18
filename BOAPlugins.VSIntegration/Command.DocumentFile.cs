using System;
using BOAPlugins.DocumentFile;

namespace BOASpSearch
{
    /// <summary>
    ///     Command handler
    /// </summary>
    internal sealed partial class Command1
    {
        #region Methods
        void DocumentFile(object sender, EventArgs e)
        {
            VisualStudio.DocumentActiveFile();
            VisualStudio.ActiveDocument_SelectAll();

            var selectedText = VisualStudio.CursorSelectedText;

            var handler = new Handler();

            var input = new Input
            {
                CSharpCode = selectedText
            };

            Result result = null;

            try
            {
                result = handler.Handle(input);
            }
            catch (Exception exception)
            {
                VisualStudio.UpdateStatusbarText(exception.Message);
                return;
            }

            if (result.ErrorMessage != null)
            {
                VisualStudio.UpdateStatusbarText(result.ErrorMessage);
                return;
            }

            VisualStudio.ActiveDocument_ReplaceText(selectedText, result.CSharpCode);

            VisualStudio.ActiveDocument_Save();

            VisualStudio.UpdateStatusbarText("Successfully documented.");
        }
        #endregion
    }
}