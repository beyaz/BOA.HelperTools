using System;

namespace BOAPlugins.FormApplicationGenerator
{
    static class DefinitionFormTsxFile
    {
        #region Public Methods
        public static string GenerateCode(Model Model)
        {
            var tsxCodeInfo = TsxCodeGeneration.EvaluateTSCodeInfo(Model,true);

            var tabFormdecleration = Model.IsTabForm ? Environment.NewLine+"this.tabEnabled = true;" : "";

            return @"

import * as React from ""react""
import { TransactionPage, TransactionPageComposer } from ""b-framework""
import { BAccountComponent } from ""b-account-component""
import { BInput } from ""b-input""
import { BComboBox } from ""b-combo-box""
import { BCheckBox } from ""b-check-box""
import { BGridSection } from ""b-grid-section""
import { BGridRow } from ""b-grid-row""
import { BInputMask } from ""b-input-mask""
import { BDateTimePicker } from ""b-datetime-picker""
import { BBranchComponent } from ""b-branch-component""
import { BParameterComponent } from ""b-parameter-component""
import { BInputNumeric } from ""b-input-numeric"";
import { BCard } from ""b-card""
import { BCardSection } from ""b-card-section""

import { Helper } from ""../utils/Helper"";
import { FormAssistant } from ""../utils/FormAssistant"";
import { Message } from ""../utils/Message"";

import Common = BOA.Common.Types;
import BasePageProps = BFramework.BasePageProps;
import " + Model.RequestNameForDefinition + @" = " + Model.NamespaceNameForType + @"." + Model.RequestNameForDefinition + @";

class CommandName
{
    static readonly New    = ""New"";
    static readonly Save    = ""Save"";
    static readonly Approve = ""Approve"";
    static readonly Reject  = ""Reject"";
}

" + tsxCodeInfo.SnapDefinition + @"

class " + Model.FormName + @"Form extends TransactionPage
{
    readonly assistant: FormAssistant<" + Model.RequestNameForDefinition + @">;

    executeWorkFlow: () => void;

    " + tsxCodeInfo.SnapDecleration + @"

    constructor(props: BasePageProps)
    {
        super(props);

        this.connect(this);

        this.assistant = FormAssistant.create(this, """ + Model.NamespaceNameForType + @"." + Model.RequestNameForDefinition + @""");
        " + tabFormdecleration + @"
    }

    evaluateActionStates()
    {
        if (this.state.pageParams.data)
        {
            this.disableAction(CommandName.New);
        }
    }

    onActionClick(command: Common.ResourceActionContract, executeWorkFlow: () => void)
    {
        let isCompleted = false;

        this.executeWorkFlow = executeWorkFlow;

        switch (command.commandName)
        {

            case CommandName.New:
            {
                this.assistant.evaluateInitialState();
                break;
            }

            case CommandName.Save:
            {
                this.assistant.sendWindowRequest(CommandName.Save);
                break;
            }
            case CommandName.Approve:
            {
                isCompleted = true;
                break;
            }
            case CommandName.Reject:
            {
                isCompleted = true;
                break;
            }
        }

        return isCompleted;
    }

    componentDidMount()
    {
        super.componentDidMount();

        this.assistant.componentDidMount();
    }

    proxyDidRespond(proxyResponse: ProxyResponse)
    {
        return this.assistant.receiveResponse(proxyResponse);
    }

    " + (Model.IsTabForm ? "renderTab" : "render") + @"()
    {
        if (!this.assistant.isReadyToRender())
        {
            return  " + (Model.IsTabForm ? "[]" : "<div/>") + @";
        }

        const state = this.state;

        const context = state.context;

        const windowRequest = this.assistant.getWindowRequest();

        const data = windowRequest.data;

        return " + (Model.IsTabForm ? "[" : "(") + @"
" + tsxCodeInfo.RenderCodeForJsx + @"
        " + (Model.IsTabForm ? "]" : ")") + @";
    }
}

export default TransactionPageComposer(" + Model.FormName + @"Form);

";
        }
        #endregion
    }
}