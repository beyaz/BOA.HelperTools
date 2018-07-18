using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Util;
using BOA.Common.Helpers;
using FeserWard.Controls;
using WhiteStone.Jaml;

namespace BOAPlugins.TypeSearchView
{
    class UserIteraction
    {
        #region Public Methods
        public static string FindType(string assemblySearchDirectoryPaths)
        {
            var view = new View
            {
                QueryProvider = new SingleColumnResultsProvider {AssemblySearchDirectoryPaths = assemblySearchDirectoryPaths}
            };
            view.ShowDialog();
            return view.SelectedTypeFullName;
        }
        #endregion
    }

    

    class SingleColumnResultsProvider : IIntelliboxResultsProvider
    {
        #region Fields
        readonly List<ITypeDefinition> _types = new List<ITypeDefinition>();
        readonly BOATypeDataProvider provider = new BOATypeDataProvider();
        #endregion

        #region Public Properties
        public string AssemblySearchDirectoryPaths { get; set; }
        #endregion

        #region Public Methods
        public IEnumerable DoSearch(string searchTerm, int maxResults, object tag)
        {
            return GetTypes(searchTerm).Where(term => term.FullName.ToUpperEN().Contains(searchTerm.ToUpperEN())).Select(t => t.FullName).Take(maxResults);
        }
        #endregion

        #region Methods
        protected virtual IEnumerable<string> Sort(IEnumerable<string> preResults)
        {
            return preResults.OrderByDescending(s => s.Length);
        }

        IReadOnlyList<ITypeDefinition> GetTypes(string searchTerm)
        {
            var types = new List<ITypeDefinition>();

            foreach (var path in AssemblySearchDirectoryPaths.Split(','))
            {
                types.AddRange(provider.GetAllTypes(path, searchTerm));
            }

            _types.AddRange(types);

            LastUsedTypes.Value = _types;

            return _types;
        }
        #endregion
    }
}