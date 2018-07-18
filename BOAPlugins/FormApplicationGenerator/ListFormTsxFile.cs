namespace BOAPlugins.FormApplicationGenerator
{
    static class ListFormTsxFile
    {
        #region Public Methods
        public static string GenerateCode(Model Model)
        {
            var tsxCode = TsxCodeGeneration.EvaluateTSCodeInfo(Model.ListFormSearchFields,false);

            return @"

import * as React from ""react""
import { BrowsePage, BrowsePageComposer } from ""b-framework""
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
import { Helper } from ""../utils/Helper"";
import { FormAssistant } from ""../utils/FormAssistant"";
import { Message } from ""../utils/Message"";
import { BFormManager } from ""b-form-manager""

import Common = BOA.Common.Types;
import BasePageProps = BFramework.BasePageProps;
import " + Model.RequestNameForList + @" = " + Model.NamespaceNameForType + @"." + Model.RequestNameForList + @";

class CommandName
{
    static readonly Clean   = ""Clean"";
    static readonly GetInfo = ""GetInfo"";
    static readonly Open    = ""Open"";
}

" + tsxCode.DefinitionCode + @"

class " + Model.FormName + @"ListForm extends BrowsePage
{
    readonly assistant: FormAssistant<" + Model.RequestNameForList + @">;

    " + tsxCode.PropertyDeclerationCode + @"

    constructor(props: BasePageProps)
    {
        super(props);

        this.connect(this);

        this.assistant = FormAssistant.create(this, """ + Model.NamespaceNameForType + @"." + Model.RequestNameForList + @""");
    }

    onRowSelectionChanged()
    {
        this.evaluateActionStates();
    }

    evaluateActionStates()
    {
        if (Helper.isOnlyOneRecordSelected(this))
        {
            this.enableAction(CommandName.Open);
        }
        else
        {
            this.disableAction(CommandName.Open);
        }
    }

    onActionClick(command: Common.ResourceActionContract)
    {
        switch (command.commandName)
        {
            case CommandName.Clean:
            {
                this.assistant.evaluateInitialState();
                break;
            }

            case CommandName.GetInfo:
            {
                this.assistant.sendWindowRequest(CommandName.GetInfo);
                break;
            }

            case CommandName.Open:
            {
                if (!Helper.isOnlyOneRecordSelected(this))
                {
                    this.showStatusMessage(Message.RecordChoosing);
                    return;
                }

                const resourceCode = ""?""; // TODO: definition form resource code expects here.
                const data = this.getSelectedRows()[0];

                BFormManager.show(resourceCode, data, /*showAsNewPage*/true);

                break;
            }


        }
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

    render()
    {
        if (!this.assistant.isReadyToRender())
        {
            return <div/>;
        }

        const state = this.state;

        const context = state.context;

        const windowRequest = this.assistant.getWindowRequest();

        const data = windowRequest.data;

        return (
            " + tsxCode.RenderCodeForJsx + @"
        );
    }
}

export default BrowsePageComposer(" + Model.FormName + @"ListForm);

";
        }
        #endregion
    }
}