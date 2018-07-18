using System;
using System.IO;
using System.Linq;
using BOA.Common.Helpers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace BOA.CodeGeneration.Util
{
    public class CheckinSolutionInput
    {
        #region Public Properties
        public string Comment          { get; set; }
        public string ResultMessage    { get; set; }
        public string SolutionFilePath { get; set; }
        #endregion
    }

    public class TFSAccessForBOA
    {
        #region Public Methods
        public void CheckinFile(string path, string comment)
        {
            var tfsServerUri = GetTfsServerPath(path);

            var pc = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(tfsServerUri));

            var workspaceInfo = Workstation.Current.GetLocalWorkspaceInfo(path);
            var workspace     = workspaceInfo?.GetWorkspace(pc);

            var change = workspace?.GetPendingChangesEnumerable().Where(p => p.LocalItem.ToUpperEN() == path.ToUpperEN()).ToArray();
            if (change?.Any() == true)
            {
                workspace.CheckIn(change.ToArray(), comment);
            }
        }

        public void CheckinSolution(CheckinSolutionInput input)
        {
            var solutionFilePath = input.SolutionFilePath;
            var comment          = input.Comment;

            var solutionFileDirectory = Path.GetDirectoryName(solutionFilePath) + Path.DirectorySeparatorChar;

            var tfsServerUri = GetTfsServerPath(solutionFilePath);

            var pc = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(tfsServerUri));

            var workspaceInfo = Workstation.Current.GetLocalWorkspaceInfo(solutionFilePath);

            var workspace = workspaceInfo?.GetWorkspace(pc);

            if (workspace == null)
            {
                input.ResultMessage = "Workspace not found. @SolutionFilePath: " + input.SolutionFilePath;
                return;
            }

            var change = workspace.GetPendingChangesEnumerable().Where(p => p.LocalItem.ToUpperEN().StartsWith(solutionFileDirectory.ToUpperEN())).ToArray();
            if (change.Any())
            {
                workspace.CheckIn(change.ToArray(), comment);

                input.ResultMessage = "Successfully checked in. @File Count: " + change.Length;
            }
            else
            {
                input.ResultMessage = "There is no pending changes in solution: " + solutionFileDirectory;
            }
        }

        public bool CheckoutFile(string path)
        {
            var ConstTfsServerUri = GetTfsServerPath(path);

            using (var pc = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(ConstTfsServerUri)))
            {
                if (pc == null)
                {
                    return false;
                }

                var workspaceInfo = Workstation.Current.GetLocalWorkspaceInfo(path);
                var workspace     = workspaceInfo?.GetWorkspace(pc);
                return workspace?.PendEdit(path, RecursionType.Full) == 1;
            }
        }
        #endregion

        #region Methods
        string GetTfsServerPath(string path)
        {
            if (path.IndexOf("WORKDE", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "http://srvtfs:8080/tfs/DE";
            }

            return "http://srvtfs:8080/tfs/KT";
        }
        #endregion
    }
}