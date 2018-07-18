using System;
using System.IO;
using System.Reflection;

namespace BOA.CodeGeneration.Util
{
    /// <summary>
    ///     Defines the boa assembly resolver.
    /// </summary>
    public static class BOAAssemblyResolver
    {
        #region Public Methods
        /// <summary>
        ///     Attaches to current domain.
        /// </summary>
        public static void AttachToCurrentDomain()
        {
            AppDomain.CurrentDomain.AssemblyResolve += DomainAssemblyResolve;
        }
        #endregion

        #region Methods
        static Assembly DomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name  = args.Name;
            var index = name.IndexOf(',');
            if (index > 0)
            {
                name = name.Substring(0, index);
            }

            var filePath = string.Format(@"D:\BOA\server\bin\{0}.dll", name);
            if (File.Exists(filePath))
            {
                return Assembly.LoadFile(filePath);
            }

            filePath = string.Format(@"D:\BOA\client\bin\{0}.dll", name);
            if (File.Exists(filePath))
            {
                return Assembly.LoadFile(filePath);
            }

            filePath = string.Format(@"D:\BOA\client\bin\en\{0}.dll", name);
            if (File.Exists(filePath))
            {
                return Assembly.LoadFile(filePath);
            }

            throw new ArgumentException("AssemblyNotFound:" + args.Name);
        }
        #endregion
    }
}