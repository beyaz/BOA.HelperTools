using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace BOA.CodeGeneration.Model
{
    public interface ITypeDefinition
    {
        #region Public Properties
        TypeDefinition                     Definition { get; }
        string                             FullName   { get; }
        string                             Name       { get; }
        IReadOnlyList<IPropertyDefinition> Properties { get; }
        #endregion
    }

    public interface IPropertyDefinition
    {
        #region Public Properties
        string Name         { get; }
        Type   PropertyType { get; }
        #endregion
    }
}