using System;
using System.Linq;

namespace BOAPlugins.FormApplicationGenerator
{
    static class TypeFileForDefinitionForm
    {
        #region Public Methods
        public static string GenerateCode(Model Model)
        {
            var fields = string.Join(Environment.NewLine, Model.FormDataClassFields.Select(fieldInfo => $"        public {fieldInfo.DotNetType.ToCSharp()} {fieldInfo.Name} " + " { get; set; }"));

            return @"

using System;
using BOA.Common.Types;

namespace " + Model.NamespaceNameForType + @"
{      

    [Serializable]
    public class " + Model.DefinitionFormDataClassName + @"
    {
" + fields + @"
    }

    [Serializable]
    public class " + Model.FormName + @"FormState : FormStateBase
    {

    }
   
    [Serializable]
    public class " + Model.FormName + @"FormDataSource
    {

    }
    
    [Serializable]
    public class " + Model.FormName + @"FormRequest : TransactionRequestBase, IWorkFlow
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref=""" + Model.FormName + @"FormRequest"" /> class.
        /// </summary>
        public " + Model.FormName + @"FormRequest()
        {
            WorkFlowData         = new WorkFlowRequestData();
            WorkFlowInternalData = new WorkFlowRequestInternalData();
        }
        #endregion

        #region Public Properties
        /// <summary>
        ///     Gets or sets the data.
        /// </summary>
        public " + Model.DefinitionFormDataClassName + @" Data { get; set; }

        /// <summary>
        ///     Gets or sets the data source.
        /// </summary>
        public " + Model.FormName + @"FormDataSource DataSource { get; set; }

        /// <summary>
        ///     Gets or sets the state.
        /// </summary>
        public " + Model.FormName + @"FormState State { get; set; }

        /// <summary>
        ///     Gets or sets the work flow data.
        /// </summary>
        public WorkFlowRequestData WorkFlowData { get; set; }

        /// <summary>
        ///     Gets or sets the work flow internal data.
        /// </summary>
        public WorkFlowRequestInternalData WorkFlowInternalData { get; set; }
        #endregion
    }
}


";
        }
        #endregion
    }
}