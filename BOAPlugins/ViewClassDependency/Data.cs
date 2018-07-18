using BOAPlugins.Models;

namespace BOAPlugins.ViewClassDependency
{
    public class Data : ResultBase
    {
        #region Public Properties
        public string ActiveProjectName { get; set; }

        public string AssemblySearchDirectoryPath { get; set; }

        public string DgmlFileContent { get; set; }
        public string OutputFileFullPath { get; set; }

        public string SelectedText { get; set; }
        #endregion
    }
}