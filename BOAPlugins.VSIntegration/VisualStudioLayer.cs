using System;
using System.IO;
using System.Linq;
using BOA.Common.Helpers;
using BOAPlugins.VSIntegration;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Window = System.Windows.Window;

namespace BOASpSearch
{
    public class VisualStudioLayer : IVisualStudioLayer
    {
        #region Fields
        readonly IServiceProvider ServiceProvider;
        #endregion

        #region Constructors
        public VisualStudioLayer(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        #endregion

        #region Public Properties
        public string ActiveDocumentText          => ActiveDocument_TextSelection?.Text;
        public string ActiveProjectCsprojFilePath => ActiveProject?.FullName;

        public string ActiveProjectName => ActiveProject?.Name;

        public string CursorSelectedText
        {
            get
            {
                if (ActiveDocument == null)
                {
                    return null;
                }

                return ActiveDocument_TextSelection?.Text;
            }
        }
        #endregion

        #region Properties
        Document ActiveDocument => DTE?.ActiveDocument;

        TextSelection ActiveDocument_TextSelection => (TextSelection) ActiveDocument?.Selection;
        Project       ActiveProject                => (Project) ActiveSolutionProjects.FirstOrDefault();

        object[] ActiveSolutionProjects
        {
            get
            {
                //grab the DTE object
                var dte = (DTE2) ServiceProvider.GetService(typeof(DTE));
                //Get the active projects within the solution.
                return dte.ActiveSolutionProjects;
            }
        }

        DTE DTE => (DTE) ServiceProvider.GetService(typeof(DTE));
        #endregion

        #region Public Methods
        public void ActiveDocument_ReplaceText(string text, string newText)
        {
            ActiveDocument?.ReplaceText(text, newText);
        }

        public bool ActiveDocument_Save()
        {
            if (ActiveDocument == null)
            {
                return false;
            }

            ActiveDocument.Saved = true;

            return true;
        }

        public void ActiveDocument_SelectAll()
        {
            ActiveDocument_TextSelection?.SelectAll();
        }

        /// <summary>
        ///     Creates the new SQL file.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="name">The name.</param>
        public void CreateNewSQLFile(string text, string name)
        {
            var _applicationObject = DTE;
            _applicationObject.ItemOperations.NewFile("General\\Sql File", name + ".sql");

            var objTextDoc   = (TextDocument) _applicationObject.ActiveDocument.Object("TextDocument");
            var objEditPoint = objTextDoc.StartPoint.CreateEditPoint();
            objEditPoint.Insert(text);
        }

        public void DocumentActiveFile()
        {
            ActiveDocument_SelectAll();
            DTE.ExecuteCommand("Tools.SubMain.GhostDoc.DocumentThis");
        }

        public string GetBinFolderPathOfActiveProject()
        {
            //loop through each active project
            foreach (Project _activeProject in ActiveSolutionProjects)
            {
                //get the directory path based on the project file.
                var _projectPath = Path.GetDirectoryName(_activeProject.FullName);
                if (_projectPath == null)
                {
                    continue;
                }

                //get the output path based on the active configuration
                var _projectOutputPath = _activeProject.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();

                while (true)
                {
                    if (_projectOutputPath.StartsWith("..\\"))
                    {
                        _projectOutputPath = _projectOutputPath.RemoveFromStart("..\\");
                        _projectPath       = Directory.GetParent(_projectPath).FullName;
                    }
                    else
                    {
                        break;
                    }
                }

                //combine the project path and output path to get the bin path
                var _projectBinPath = Path.Combine(_projectPath, _projectOutputPath);

                //if the directory exists (already built) then open that directory
                //in windows explorer using the diagnostics.process object
                if (Directory.Exists(_projectBinPath))
                {
                    return _projectBinPath;
                }
            }

            return null;
        }

        public string GetSolutionFilePath()
        {
            var dte = DTE;
            return dte.Solution.FullName;
        }

        public void OpenFile(string filePath)
        {
            DTE.ItemOperations.OpenFile(filePath);
        }

        public void ShowDialog(Window window)
        {
            var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

            uiShell.EnableModeless(0);

            window.ShowDialog();

            uiShell.EnableModeless(1);
        }

        /// <summary>
        ///     Traces the specified message.
        /// </summary>
        public void UpdateStatusbarText(string message)
        {
            var statusBar = Package.GetGlobalService(typeof(IVsStatusbar)) as IVsStatusbar;
            statusBar?.SetText(message);
        }
        #endregion
    }
}