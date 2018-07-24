using System.Collections.Generic;
using System.Linq;
using BOA.Common.Helpers;
using BOAPlugins.ExportingModel;

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
        public static TsxCodeInfo EvaluateTSCodeInfo(List<FieldInfo> FormDataClassFields, bool isDefinitionForm)
        {
            var renderInGroupBox = isDefinitionForm;

            NamingHelper.InitializeFieldComponentTypes(FormDataClassFields);

            var PropertyDeclerationCode = "";

            var snapDefinitionCode = new PaddedStringBuilder();

            var HasSnap = FormDataClassFields.Any(x => x.HasSnapName());
            if (HasSnap)
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

                PropertyDeclerationCode = "snaps: ISnaps;";
            }

            var DefinitionCode = snapDefinitionCode.ToString();

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

                RenderComponent(renderCodes, dataField);

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

            return new TsxCodeInfo
            {
                HasSnap                 = HasSnap,
                PropertyDeclerationCode = PropertyDeclerationCode,
                DefinitionCode          = DefinitionCode,
                RenderCodeForJsx        = renderCodes.ToString()
            };
        }
        #endregion

        #region Methods
        static void RenderComponent(PaddedStringBuilder output, FieldInfo dataField)
        {
            var valueAccessPath = Exporter.GetResolvedPropertyName(dataField.Name);

            if (dataField.ComponentName == ComponentName.BDateTimePicker)
            {
                output.AppendLine("<BDateTimePicker format = \"DDMMYYYY\"");
                output.AppendLine("                 value = {data." + valueAccessPath + "}");
                output.AppendLine("                 dateOnChange = {(e: any, value: Date) => data." + valueAccessPath + " = value}");
                output.AppendLine("                 floatingLabelTextDate = {Message." + dataField.Name + "}");
                output.AppendLine("                 context = {context}/>");
                return;
            }

            if (dataField.ComponentName == ComponentName.BInput)
            {
                output.AppendLine("<BInput value = {data." + valueAccessPath + "}");
                output.AppendLine("        onChange = {(e: any, value: string) => data." + valueAccessPath + " = value}");
                output.AppendLine("        floatingLabelText = {Message." + dataField.Name + "}");
                output.AppendLine("        context = {context}/>");
                return;
            }

            if (dataField.ComponentName == ComponentName.BInputNumeric)
            {
                output.AppendLine("<BInputNumeric value = {data." + valueAccessPath + "}");
                output.AppendLine("               onChange = {(e: any, value: any) => data." + valueAccessPath + " = value}");
                output.AppendLine("               floatingLabelText = {Message." + dataField.Name + "}");
                if (dataField.TypeName == DotNetTypeName.Decimal)
                {
                    output.AppendLine("               format = {\"D\"}");
                    output.AppendLine("               maxLength = {22}");
                }
                else
                {
                    output.AppendLine("               maxLength = {10}");
                }

                output.AppendLine("               context = {context}/>");
                return;
            }

            if (dataField.ComponentName == ComponentName.BAccountComponent)
            {
                output.AppendLine("<BAccountComponent accountNumber = {data." + valueAccessPath + "}");
                output.AppendLine("                   onAccountSelect = {(selectedAccount: any) => data." + valueAccessPath + " = selectedAccount ? selectedAccount.accountNumber : null}");
                output.AppendLine("                   isVisibleBalance={false}");
                output.AppendLine("                   isVisibleAccountSuffix={false}");
                output.AppendLine("                   enableShowDialogMessagesInCallback={false}");
                output.AppendLine("                   isVisibleIBAN={false}");
                output.AppendLine("                   ref={(r: any) => this.snaps." + dataField.GetSnapName() + " = r}");
                output.AppendLine("                   context = {context}/>");
                return;
            }

            if (dataField.ComponentName == ComponentName.BCheckBox)
            {
                output.AppendLine("<BCheckBox checked = {data." + valueAccessPath + "}");
                output.AppendLine("           onCheck = {(e: Object, isChecked: boolean) => data." + valueAccessPath + " = isChecked}");
                output.AppendLine("           label   = {Message." + dataField.Name + "}");
                output.AppendLine("           context = {context}/>");
                return;
            }

            if (dataField.ComponentName == ComponentName.BParameterComponent)
            {
                if (dataField.TypeName == DotNetTypeName.Int32)
                {
                    output.AppendLine("<BParameterComponent selectedParamCode = {Helper.numberToString(data." + valueAccessPath + ")}");
                    output.AppendLine("                     onParameterSelect = {(selectedParameter: BOA.Types.Kernel.General.ParameterContract) => data." + valueAccessPath + " = selectedParameter ? Helper.stringToNumber(selectedParameter.paramCode) : null}");
                }
                else
                {
                    output.AppendLine("<BParameterComponent selectedParamCode = {data." + valueAccessPath + "}");
                    output.AppendLine("                     onParameterSelect = {(selectedParameter: BOA.Types.Kernel.General.ParameterContract) => data." + valueAccessPath + " = selectedParameter ? selectedParameter.paramCode : null}");
                }

                if (dataField.ParamType.IsNullOrWhiteSpace())
                {
                    output.AppendLine("                     paramType=\"GENDER\"");
                }
                else
                {
                    output.AppendLine("                     paramType=\"" + dataField.ParamType + "\"");
                }

                output.AppendLine("                     hintText = {Message." + dataField.Name + "}");
                output.AppendLine("                     labelText = {Message." + dataField.Name + "}");
                output.AppendLine("                     isAllOptionIncluded={true}");
                output.AppendLine("                     paramColumns={[");
                output.AppendLine("                            { name: \"paramCode\",        header: Message.Code,        visible: false },");
                output.AppendLine("                            { name: \"paramDescription\", header: Message.Description, width:   200 }");
                output.AppendLine("                     ]}");
                output.AppendLine("                     ref={(r: any) => this.snaps." + dataField.GetSnapName() + " = r}");
                output.AppendLine("                     context = {context}/>");
                return;
            }

            if (dataField.ComponentName == ComponentName.BBranchComponent)
            {
                output.AppendLine("<BBranchComponent selectedBranchId = {data." + valueAccessPath + "}");
                output.AppendLine("                  onBranchSelect = {(selectedBranch: BOA.Common.Types.BranchContract) => data." + valueAccessPath + " = selectedBranch ? selectedBranch.branchId : null}");
                output.AppendLine("                  mode={\"horizontal\"}");
                output.AppendLine("                  labelText = {Message." + dataField.Name + "}");
                output.AppendLine("                  sortOption={BBranchComponent.name}");
                output.AppendLine("                  context = {context}/>");
            }
        }
        #endregion
    }
}