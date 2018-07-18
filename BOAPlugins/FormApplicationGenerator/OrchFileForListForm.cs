using System;
using System.Linq;

namespace BOAPlugins.FormApplicationGenerator
{
    static class OrchFileForListForm
    {
        #region Public Methods
        public static string GenerateCode(Model Model)
        {
            var resultGridColumnNames = string.Join(","+Environment.NewLine , Model.FormDataClassFields.Select(fieldInfo => $"nameof({Model.DefinitionFormDataClassName}.{fieldInfo.Name})"));

            return @"
using BOA.Base;
using BOA.Common.Extensions;
using BOA.Common.Types;
using BOA.Common.Helpers;
using " + Model.NamespaceNameForType + @";

namespace " + Model.NamespaceNameForOrch + @"
{   
    public class " + Model.FormName + @"ListForm
    {
        /// <summary>
        ///     Evaluates the initial state.
        /// </summary>
        public GenericResponse<" + Model.RequestNameForList + @"> EvaluateInitialState(" + Model.RequestNameForList + @" request, ObjectHelper objectHelper)
        {
            return GetService(request, objectHelper).EvaluateInitialState();
        }

        /// <summary>
        ///     Gets the information.
        /// </summary>
        public GenericResponse<" + Model.RequestNameForList + @"> GetInfo(" + Model.RequestNameForList + @" request, ObjectHelper objectHelper)
        {
            return GetService(request, objectHelper).GetInfo();
        }

        /// <summary>
        ///     Gets the service.
        /// </summary>
        static Service GetService(" + Model.RequestNameForList + @" request, ObjectHelper objectHelper)
        {
            return new Service
            {
                Request = request,
                Context = objectHelper.Context
            };
        }


        /// <summary>
        ///     The service
        /// </summary>
        class Service : ProcessBase
        {
            #region Public Properties
            /// <summary>
            ///     Gets or sets the request.
            /// </summary>
            public " + Model.RequestNameForList + @" Request { get; set; }
            #endregion

            #region Public Methods
            /// <summary>
            ///     Evaluates the initial state.
            /// </summary>
            public GenericResponse<" + Model.RequestNameForList + @"> EvaluateInitialState()
            {
                var returnObject = CreateResponse(Request);

                Request.DataSource.DataGridInfo = GetDataGridInfo();
             
                return returnObject;
            }

            /// <summary>
            ///     Gets the information.
            /// </summary>
            public GenericResponse<" + Model.RequestNameForList + @"> GetInfo()
            {
                var returnObject = CreateResponse(Request);

                #region TODO: Expects code
                Request.DataSource.Records = RandomValue.ListOf<" + Model.DefinitionFormDataClassName + @">();
                #endregion

                Request.State.StatusMessage = Request.DataSource.Records.Count + Message.RecordsWereBrought;

                return returnObject;
            }
            #endregion

            #region Methods
            /// <summary>
            ///     Gets the data grid information.
            /// </summary>
            static DataGridInfo GetDataGridInfo()
            {
                return DataGridInfo.Create(typeof(" + Model.DefinitionFormDataClassName + @"), new[]
                {
                    " + resultGridColumnNames + @"
                });
            }
            #endregion

        }
    }
}
";
        }
        #endregion
    }
}