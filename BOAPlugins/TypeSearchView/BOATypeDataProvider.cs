using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Util;
using BOAPlugins.Models;
using Mono.Cecil;

namespace BOAPlugins.TypeSearchView
{
    public class BOATypeDataProvider
    {
        #region Fields
        readonly Dictionary<string, bool> _loadedAssemblyNames = new Dictionary<string, bool>();
        #endregion

        #region Public Properties
        public int CountTreshold { get; set; } = 20;

        public string SearchDirectory { get; set; }
        #endregion

        #region Public Methods
        public IReadOnlyList<ITypeDefinition> GetAllTypes(string searchDirectory = @"d:\boa\server\bin\", string fullTypeNameStartsWith = null)
        {
            var list = new List<ITypeDefinition>();
            const char dot = '.';

            string assemblyNameStartsWith = null;
            var arr = fullTypeNameStartsWith?.Split(dot);
            if (arr?.Length > 2)
            {
                assemblyNameStartsWith = string.Join(dot.ToString(), arr.Take(3));
            }

            const int maxMB = 2 * 1024 * 1024;

            var count = 0;
            foreach (var fileInfo in Directory.EnumerateFiles(searchDirectory)
                                              .Where(f => f.EndsWith(".dll") || f.EndsWith(".exe"))
                                              .Select(f => new FileInfo(f))
                                              .Where(f => f.Name.StartsWith("BOA.") && f.Length < maxMB || f.Length < 250 * 1024)
                                              .OrderBy(f => f.Length))
            {
                var fullName = fileInfo.FullName;

                if (assemblyNameStartsWith != null)
                {
                    if (!fileInfo.Name.StartsWith(assemblyNameStartsWith, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }
                else
                {
                    if (count > CountTreshold)
                    {
                        return list;
                    }
                }

                if (_loadedAssemblyNames.ContainsKey(fileInfo.FullName))
                {
                    continue;
                }

                _loadedAssemblyNames[fileInfo.FullName] = true;

                AssemblyDefinition assembly = null;

                try
                {
                    var resolver = new DefaultAssemblyResolver();
                    resolver.AddSearchDirectory(BinFolderPaths.ServerBin);
                    resolver.AddSearchDirectory(BinFolderPaths.ClientBin);

                    assembly = AssemblyDefinition.ReadAssembly(fullName, new ReaderParameters {AssemblyResolver = resolver});
                }
                catch (BadImageFormatException)
                {
                    continue;
                }

                count++;
                foreach (var moduleDefinition in assembly.Modules)
                {
                    foreach (var type in moduleDefinition.Types)
                    {
                        if (type.Name.Contains("<"))
                        {
                            continue;
                        }
                        list.Add(type.ToITypeDefinition());
                    }
                }
            }

            return list;
        }
        #endregion
    }
}