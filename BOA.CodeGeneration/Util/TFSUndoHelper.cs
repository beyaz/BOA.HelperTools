using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace BOA.CodeGeneration.Util
{
    public static class TFSUndoHelper
    {
        #region Public Methods
        public static void UncheckoutIfItHasBeenNotModified(string fileName, IServiceProvider serviceProvider)
        {
            var _md5Provider = new MD5CryptoServiceProvider();

            var fileInfo = new FileInfo(fileName);
            if (fileInfo.IsReadOnly)
            {
                return;
            }

            var tfsContext = serviceProvider.GetService<ITeamFoundationContextManager>();
            if (tfsContext == null)
            {
                return;
            }

            if (tfsContext.CurrentContext == null || tfsContext.CurrentContext.TeamProjectCollection == null)
            {
                return;
            }

            var tfs = tfsContext.CurrentContext.TeamProjectCollection;
            var vcs = tfs.GetService<VersionControlServer>();
            if (vcs == null)
            {
                return;
            }

            // If file is new nothing comparer
            if (!vcs.ServerItemExists(fileName, ItemType.File))
            {
                return;
            }

            var workspace = vcs.TryGetWorkspace(fileName);
            if (workspace == null)
            {
                return;
            }

            var fileInfoItem = vcs.GetItem(fileName);
            if (fileInfoItem != null)
            {
                using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
                {
                    var currentHash = _md5Provider.ComputeHash(fileStream);
                    var hashEquals  = HashEquals(currentHash, fileInfoItem.HashValue);
                    if (hashEquals && !IsMerged(workspace, fileName))
                    {
                        // This is from
                        // Assembly: Microsoft.VisualStudio.TeamFoundation.VersionControl, Version=12.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
                        // Type: Microsoft.VisualStudio.TeamFoundation.VersionControl.ClientHelperVS
                        // Method: internal static void Undo(Workspace workspace, PendingChange[] changes)
                        using (new WorkspaceSuppressAsynchronousScanner(workspace))
                        {
                            using (new WorkspacePersistedMetadataTables(workspace))
                            {
                                workspace.Undo(ItemSpec.FromStrings(new[] {fileInfoItem.ServerItem}, RecursionType.None), false);
                                workspace.UnqueueForEdit(fileName);

                                ////workspace.Get(new GetRequest(fileInfoItem.ServerItem, RecursionType.None, VersionSpec.Latest), GetOptions.None);
                                //var vsFileChangeEx = GetService<Microsoft.VisualStudio.Shell.Interop.SVsFileChangeEx, Microsoft.VisualStudio.Shell.Interop.IVsFileChangeEx>();
                                //vsFileChangeEx.SyncFile(fileName);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Methods
        static bool HashEquals(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length)
            {
                return false;
            }

            for (var i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                {
                    return false;
                }
            }

            return true;
        }

        static bool IsMerged(Workspace workspace, string fileName)
        {
            var pendingChanges = workspace.GetPendingChanges(fileName);
            if (pendingChanges != null && pendingChanges.Length > 0)
            {
                return pendingChanges[0].IsMerge;
            }

            return false;
        }
        #endregion
    }
}