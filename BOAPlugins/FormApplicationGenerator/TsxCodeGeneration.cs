using System.Collections.Generic;
using System.Linq;
using BOA.Common.Helpers;

namespace BOAPlugins.FormApplicationGenerator
{
    class TsxCodeInfo
    {
        #region Public Properties
        public string DefinitionCode          { get; set; }
        public bool   HasSnap                 { get; set; }
        public string PropertyDeclerationCode { get; set; }
        public string RenderCodeForJsx        { get; set; }
        #endregion
    }

    static class TsxCodeGeneration
    {
        #region Public Methods
        public static TsxCodeInfo EvaluateTSCodeInfo(List<FieldInfo> FormDataClassFields,bool isDefinitionForm)
        {
            var renderInGroupBox = isDefinitionForm;

            NamingHelper.InitializeFieldComponentTypes(FormDataClassFields);

            var result = new TsxCodeInfo
            {
                PropertyDeclerationCode = ""
            };

            var snapDefinitionCode = new PaddedStringBuilder();

            

            result.HasSnap = FormDataClassFields.Any(x => x.HasSnapName());
            if (result.HasSnap)
            {
                snapDefinitionCode.AppendLine("interface ISnaps");
                snapDefinitionCode.AppendLine("{");
                snapDefinitionCode.PaddingCount++;

                foreach (var dataField in FormDataClassFields.Where(x => x.HasSnapName()))
                {
                    snapDefinitionCode.AppendLine($"{dataField.GetSnapName()}: {dataField.ComponentName};");
                }

                snapDefinitionCode.PaddingCount--;
                snapDefinitionCode.AppendLine("}");

                result.PropertyDeclerationCode = "snaps: ISnaps;";
            }

            result.DefinitionCode = snapDefinitionCode.ToString();

            var renderCodes = new PaddedStringBuilder
            {
                PaddingLength = 4,
                PaddingCount  = 3
            };

            if (renderInGroupBox)
            {
                renderCodes.AppendLine("<BCardSection context={context} thresholdColumnCount={3}>");
                renderCodes.AppendLine("<BCard context={context} title={Message.?} column={0}>");
            }
            else
            {
                renderCodes.AppendLine("<BGridSection context={context}>");
            }
            
            renderCodes.PaddingCount++;

            foreach (var dataField in FormDataClassFields)
            {
                if (!renderInGroupBox)
                {
                    renderCodes.AppendLine("<BGridRow context={context}>");
                }
                    
                renderCodes.PaddingCount++;

                if (dataField.ComponentName == ComponentName.BDateTimePicker)
                {
                    renderCodes.AppendLine("<BDateTimePicker format = \"DDMMYYYY\"");
                    renderCodes.AppendLine("                 value = {data." + dataField.Name.MakeLowerCaseFirstChar() + "}");
                    renderCodes.AppendLine("                 dateOnChange = {(e: any, value: Date) => data." + dataField.Name.MakeLowerCaseFirstChar() + " = value}");
                    renderCodes.AppendLine("                 floatingLabelTextDate = {Message." + dataField.Name + "}");
                    renderCodes.AppendLine("                 context = {context}/>");
                }
                else if (dataField.ComponentName == ComponentName.BInput)
                {
                    renderCodes.AppendLine("<BInput value = {data." + dataField.Name.MakeLowerCaseFirstChar() + "}");
                    renderCodes.AppendLine("        onChange = {(e: any, value: string) => data." + dataField.Name.MakeLowerCaseFirstChar() + " = value}");
                    renderCodes.AppendLine("        floatingLabelText = {Message." + dataField.Name + "}");
                    renderCodes.AppendLine("        context = {context}/>");
                }
                else if (dataField.ComponentName == ComponentName.BInputNumeric)
                {
                    renderCodes.AppendLine("<BInputNumeric value = {data." + dataField.Name.MakeLowerCaseFirstChar() + "}");
                    renderCodes.AppendLine("               onChange = {(e: any, value: any) => data." + dataField.Name.MakeLowerCaseFirstChar() + " = value}");
                    renderCodes.AppendLine("               floatingLabelText = {Message." + dataField.Name + "}");
                    if (dataField.TypeName == DotNetTypeName.Decimal )
                    {
                        renderCodes.AppendLine("               format = {\"D\"}");
                        renderCodes.AppendLine("               maxLength = {22}");
                    }
                    else
                    {
                        renderCodes.AppendLine("               maxLength = {10}");
                    }

                    renderCodes.AppendLine("               context = {context}/>");
                }
                else if (dataField.ComponentName == ComponentName.BAccountComponent)
                {
                    renderCodes.AppendLine("<BAccountComponent accountNumber = {data." + dataField.Name.MakeLowerCaseFirstChar() + "}");
                    renderCodes.AppendLine("                   onAccountSelect = {(selectedAccount: any) => data." + dataField.Name.MakeLowerCaseFirstChar() + " = selectedAccount ? selectedAccount.accountNumber : null}");
                    renderCodes.AppendLine("                   isVisibleBalance={false}");
                    renderCodes.AppendLine("                   isVisibleAccountSuffix={false}");
                    renderCodes.AppendLine("                   enableShowDialogMessagesInCallback={false}");
                    renderCodes.AppendLine("                   isVisibleIBAN={false}");
                    renderCodes.AppendLine("                   ref={(r: any) => this.snaps."+ dataField.GetSnapName() + " = r}");
                    renderCodes.AppendLine("                   context = {context}/>");
                }
                else if (dataField.ComponentName == ComponentName.BCheckBox)
                {
                    renderCodes.AppendLine("<BCheckBox checked = {data." + dataField.Name.MakeLowerCaseFirstChar() + "}");
                    renderCodes.AppendLine("           onCheck = {(e: Object, isChecked: boolean) => data." + dataField.Name.MakeLowerCaseFirstChar() + " = isChecked}");
                    renderCodes.AppendLine("           label   = {Message." + dataField.Name + "}");
                    renderCodes.AppendLine("           context = {context}/>");
                }

                else if (dataField.ComponentName == ComponentName.BParameterComponent)
                {
                    if (dataField.TypeName == DotNetTypeName.Int32)
                    {
                        renderCodes.AppendLine("<BParameterComponent selectedParamCode = {Helper.numberToString(data." + dataField.Name.MakeLowerCaseFirstChar() + ")}");
                        renderCodes.AppendLine("                     onParameterSelect = {(selectedParameter: BOA.Types.Kernel.General.ParameterContract) => data." + dataField.Name.MakeLowerCaseFirstChar() + " = selectedParameter ? Helper.stringToNumber(selectedParameter.paramCode) : null}");
                    }
                    else
                    {
                        renderCodes.AppendLine("<BParameterComponent selectedParamCode = {data." + dataField.Name.MakeLowerCaseFirstChar() + "}");
                        renderCodes.AppendLine("                     onParameterSelect = {(selectedParameter: BOA.Types.Kernel.General.ParameterContract) => data." + dataField.Name.MakeLowerCaseFirstChar() + " = selectedParameter ? selectedParameter.paramCode : null}");
                    }

                    if (dataField.ParamType.IsNullOrWhiteSpace())
                    {
                        renderCodes.AppendLine("                     paramType=\"GENDER\"");
                    }
                    else
                    {
                        renderCodes.AppendLine("                     paramType=\""+ dataField.ParamType + "\"");
                    }
                    
                    renderCodes.AppendLine("                     hintText = {Message." + dataField.Name + "}");
                    renderCodes.AppendLine("                     labelText = {Message." + dataField.Name + "}");
                    renderCodes.AppendLine("                     isAllOptionIncluded={true}");
                    renderCodes.AppendLine("                     paramColumns={[");
                    renderCodes.AppendLine("                            { name: \"paramCode\",        header: Message.Code,        visible: false },");
                    renderCodes.AppendLine("                            { name: \"paramDescription\", header: Message.Description, width:   200 }");
                    renderCodes.AppendLine("                     ]}");
                    renderCodes.AppendLine("                     ref={(r: any) => this.snaps." + dataField.GetSnapName() + " = r}");
                    renderCodes.AppendLine("                     context = {context}/>");
                }

                else if (dataField.ComponentName == ComponentName.BBranchComponent)
                {
                    renderCodes.AppendLine("<BBranchComponent selectedBranchId = {data." + dataField.Name.MakeLowerCaseFirstChar() + "}");
                    renderCodes.AppendLine("                  onBranchSelect = {(selectedBranch: BOA.Common.Types.BranchContract) => data." + dataField.Name.MakeLowerCaseFirstChar() + " = selectedBranch ? selectedBranch.branchId : null}");
                    renderCodes.AppendLine("                  mode={\"horizontal\"}");
                    renderCodes.AppendLine("                  labelText = {Message." + dataField.Name + "}");
                    renderCodes.AppendLine("                  sortOption={BBranchComponent.name}");
                    renderCodes.AppendLine("                  context = {context}/>");
                }



                renderCodes.PaddingCount--;

                if (!renderInGroupBox)
                {
                    renderCodes.AppendLine("</BGridRow>");
                }
                
            }

            renderCodes.PaddingCount--;



            if (renderInGroupBox)
            {
                renderCodes.AppendLine("</BCard>");
                renderCodes.AppendLine("</BCardSection>");
            }
            else
            {
                renderCodes.AppendLine("</BGridSection>");
            }
            

            result.RenderCodeForJsx = renderCodes.ToString();

            return result;
        }
        #endregion
    }
}