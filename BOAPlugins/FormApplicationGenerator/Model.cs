using System;
using System.Collections.Generic;
using System.IO;
using BOA.Common.Helpers;

namespace BOAPlugins.FormApplicationGenerator
{
    [Serializable]
    public class Model
    {
        #region Public Properties
        public string          DefinitionFormDataClassName { get; set; }
        public List<FieldInfo> FormDataClassFields         { get; set; } = new List<FieldInfo>();

        public string FormName { get; set; }

        public List<FieldInfo> ListFormSearchFields { get; set; } = new List<FieldInfo>();
        public string          NamespaceName                         { get; set; }
        public string          NamespaceNameForOrch                  { get; set; }
        public string          NamespaceNameForType                  { get; set; }
        public string          RequestNameForDefinition              { get; set; }
        public string          RequestNameForList                    { get; set; }
        public string          SolutionFilePath                      { get; set; }
        #endregion
    }

    static class NamingHelper
    {
        #region Public Methods
        public static void InitializeFieldComponentTypes(List<FieldInfo> FormDataClassFields)
        {
            foreach (var field in FormDataClassFields)
            {
                if (field.ComponentName != null)
                {
                    continue;
                }

                var fieldTypeName = field.TypeName;
                if (fieldTypeName ==  DotNetTypeName.String)
                {
                    field.ComponentName = ComponentName.BInput;
                    continue;
                }

                if (fieldTypeName == DotNetTypeName.Int32 ||
                    fieldTypeName == DotNetTypeName.Decimal)
                {
                    field.ComponentName = ComponentName.BInputNumeric;
                    continue;
                }

                if (fieldTypeName == DotNetTypeName.DateTime)
                {
                    field.ComponentName = ComponentName.BDateTimePicker;
                }
            }
        }

        public static void InitializeNames(this Model Model)
        {
            var fileName      = Path.GetFileName(Model.SolutionFilePath);
            var namespaceName = fileName.RemoveFromStart("BOA.").RemoveFromEnd(".sln");

            Model.NamespaceName               = namespaceName;
            Model.RequestNameForDefinition    = Model.FormName + "FormRequest";
            Model.RequestNameForList          = Model.FormName + "ListFormRequest";
            Model.NamespaceNameForType        = "BOA.Types." + Model.NamespaceName;
            Model.NamespaceNameForOrch        = "BOA.Orchestration." + Model.NamespaceName;
            Model.DefinitionFormDataClassName = Model.FormName + "FormData";
        }
        #endregion
    }

    [Serializable]
    public class FieldInfo
    {
        #region Public Properties
        public ComponentName? ComponentName { get; set; } = FormApplicationGenerator.ComponentName.BInput;
        public string Name          { get; set; }
        public DotNetTypeName TypeName { get; set; } = DotNetTypeName.String;


        public string ParamType { get; set; }
        #endregion
    }
    [Serializable]
    public enum ComponentName
    {
        BAccountComponent,
        BDateTimePicker,
        BInput,
        BInputNumeric,
        BParameterComponent,
        BBranchComponent
    }


    [Serializable]
    public enum DotNetTypeName
    {
        Int32,
        String,
        Decimal,
        DateTime,
        Boolean
    }
}