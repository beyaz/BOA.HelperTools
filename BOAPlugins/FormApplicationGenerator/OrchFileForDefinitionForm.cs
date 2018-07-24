namespace BOAPlugins.FormApplicationGenerator
{
    static class OrchFileForDefinitionForm
    {
        #region Public Methods
        public static string GenerateCode(Model Model)
        {
            return @"

using BOA.Base;
using BOA.Common.Types;
using BOA.Common.Helpers;
using " + Model.NamespaceNameForType + @";

namespace " + Model.NamespaceNameForOrch + @"
{    
    public class " + Model.FormName + @"Form
    {
        #region Public Methods
        /// <summary>
        ///     Ends the of workflow.
        /// </summary>
        public WorkflowResponse<" + Model.RequestNameForDefinition + @"> EndOfWorkflow(" + Model.RequestNameForDefinition + @" request, ObjectHelper objectHelper)
        {
            return GetService(request, objectHelper).EndOfWorkflow();
        }

        /// <summary>
        ///     Evaluates the initial state.
        /// </summary>
        public WorkflowResponse<" + Model.RequestNameForDefinition + @"> EvaluateInitialState(" + Model.RequestNameForDefinition + @" request, ObjectHelper objectHelper)
        {
            return GetService(request, objectHelper).EvaluateInitialState();
        }

        /// <summary>
        ///     Saves the specified request.
        /// </summary>
        public WorkflowResponse<" + Model.RequestNameForDefinition + @"> Save(" + Model.RequestNameForDefinition + @" request, ObjectHelper objectHelper)
        {
            return GetService(request, objectHelper).Save();
        }
        
        #endregion

        #region Methods
        /// <summary>
        ///     Gets the service.
        /// </summary>
        static Service GetService(" + Model.RequestNameForDefinition + @" request, ObjectHelper objectHelper)
        {
            return new Service
            {
                Request = request,
                Context = objectHelper.Context
            };
        }
        #endregion

        /// <summary>
        ///     The service
        /// </summary>
        class Service : ProcessBase
        {
            #region Public Properties
            /// <summary>
            ///     Gets or sets the request.
            /// </summary>
            public " + Model.RequestNameForDefinition + @" Request { get; set; }
            #endregion

            #region Public Methods
            /// <summary>
            ///     Ends the of workflow.
            /// </summary>
            public WorkflowResponse<" + Model.RequestNameForDefinition + @"> EndOfWorkflow()
            {
                var returnObject = CreateWorkflowResponse(Request);

                // TODO: Expects code. ( workflow işleminin en sonunda yapılması gereken işlem burada yazılabilir)

                return returnObject;
            }

            /// <summary>
            ///     Evaluates the initial state.
            /// </summary>
            public WorkflowResponse<" + Model.RequestNameForDefinition + @"> EvaluateInitialState()
            {
                var returnObject = CreateWorkflowResponse(Request);
                 
                if (Request.State.IsOpenByWorkflow)
                {
                    return returnObject;
                }

                #region TODO: Expects code
                //      default atanması gereken form değerleri 
                //      enable readonly gibi state işlemleri
                //      data source ların dolsurulması mesela özel bir combonun datasource bilgisi
                //      gibi bilgiler burada doldurulmalıdır.

                Request.Data = RandomValue.Object<" + Model.DefinitionFormDataClassName + @">();
                Request.MethodName = nameof(EndOfWorkflow);
                #endregion
            
                return returnObject;
            }

            /// <summary>
            ///     Saves this instance.
            /// </summary>
            public WorkflowResponse<" + Model.RequestNameForDefinition + @"> Save()
            {
                var returnObject = CreateWorkflowResponse(Request);

                #region TODO: Expects code
                // ?
                #endregion

                Request.State.StatusMessage = Message.TransactionSaved;

                return returnObject;
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