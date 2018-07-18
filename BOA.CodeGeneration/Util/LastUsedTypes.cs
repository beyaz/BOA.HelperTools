using System.Collections.Generic;
using BOA.CodeGeneration.Model;

namespace BOA.CodeGeneration.Util
{
    public static class LastUsedTypes
    {
        #region Public Properties
        public static IReadOnlyList<ITypeDefinition> Value { get; set; }
        #endregion
    }
}