using System;
using System.Linq;

namespace BOAPlugins.FormApplicationGenerator
{
    static class TypeFileForListForm
    {
        #region Public Methods
        public static string GenerateCode(Model Model)
        {
            var searchFields = string.Join(Environment.NewLine, Model.ListFormSearchFields.Select(fieldInfo => $"        public {fieldInfo.DotNetType.ToCSharp()} {fieldInfo.Name} " + " { get; set; }"));

            return @"

using System;
using System.Collections.Generic;
using BOA.Common.Types;

namespace " + Model.NamespaceNameForType + @"
{
    
    [Serializable]
    public class " + Model.FormName + @"ListFormState : FormStateBase
    {

    }
    
    [Serializable]
    public class " + Model.FormName + @"ListFormData
    {
" + searchFields + @"
    }

    
    [Serializable]
    public class " + Model.FormName + @"ListFormDataSource : ListFormDataSourceBase
    {
        #region Public Properties
        /// <summary>
        ///     Gets or sets the records.
        /// </summary>
        public IReadOnlyCollection<" + Model.DefinitionFormDataClassName + @"> Records { get; set; }
        #endregion
    }
   
    [Serializable]
    public class " + Model.FormName + @"ListFormRequest : RequestBase
    {
        #region Public Properties
        /// <summary>
        ///     Gets or sets the data.
        /// </summary>
        public " + Model.FormName + @"ListFormData Data { get; set; }

        /// <summary>
        ///     Gets or sets the data source.
        /// </summary>
        public " + Model.FormName + @"ListFormDataSource DataSource { get; set; }

        /// <summary>
        ///     Gets or sets the state.
        /// </summary>
        public " + Model.FormName + @"ListFormState State { get; set; }
        #endregion
    }
}



";
        }
        #endregion
    }
}