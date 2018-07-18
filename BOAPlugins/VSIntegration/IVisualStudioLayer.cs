using System.Windows;

namespace BOAPlugins.VSIntegration
{
    public interface IVisualStudioLayer
    {
        #region Public Properties
        string ActiveProjectCsprojFilePath { get; }
        string ActiveProjectName { get; }
        string CursorSelectedText { get; }
        #endregion

        #region Public Methods
        void ActiveDocument_ReplaceText(string text, string newText);

        bool ActiveDocument_Save();
        void ActiveDocument_SelectAll();
        void CreateNewSQLFile(string text, string name);
        string GetBinFolderPathOfActiveProject();
        string GetSolutionFilePath();
        void OpenFile(string filePath);
        void ShowDialog(Window window);
        void UpdateStatusbarText(string message);

        void DocumentActiveFile();
        #endregion
    }
}