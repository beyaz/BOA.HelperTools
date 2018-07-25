using static BOAPlugins.FormApplicationGenerator.FieldName;
using static BOAPlugins.FormApplicationGenerator.DotNetType;

namespace BOAPlugins.FormApplicationGenerator
{
    public class VisaFee : Model
    {



       


        #region Constructors
        public VisaFee() : base(SolutionFile.CardPaymentSystem_Clearing, nameof(VisaFee))
        {
            IsTabForm = true;

            Tabs = new[]
            {
                new BTab(Amounts, new[]
                {
                    new BField(DateTime, TransactionDate),
                    new BField(Decimal, FeeAmount),
                    new BField(Int32, FeeAmountCurrency),
                    new BField(Decimal, SourceAmount),
                    new BField(Int32, SourceAmountCurrency)
                }),

                new BTab(GeneralInformation, new[]
                {
                    new BField(String, TransactionCode)
                    {
                        ComponentType = ComponentType.BParameterComponent
                    },
                    new BField(String, UsageCode)
                    {
                        ComponentType = ComponentType.BParameterComponent
                    },
                    new BField(String, CardType)
                    {
                        ComponentType = ComponentType.BParameterComponent
                    },
                    new BField(String, SourceBIN),
                    new BField(String, DestinationBIN),
                    new BField(String, Direction),
                    new BField(String, CardNumber),

                    new BField(DateTime, ClearingDate),
                    new BField(String, ClearingStatus)
                    {
                        ComponentType = ComponentType.BParameterComponent
                    }
                }),

                new BTab(ReasonInformation, new[]
                {
                    new BField(Int32, ReasonCode),
                    new BField(Int32, CountryCode),
                    new BField(DateTime, Message)
                }),

                new BTab(Status, new[]
                {
                    new BField(Boolean, Incoming),
                    new BField(Boolean, FieldName.Outgoing),
                    new BField(String, SettlementFlag),
                    new BField(String, Reimbursement),
                    new BField(String, EndOfDay),
                    new BField(String, Validation),
                    new BField(String, ValidationDefinition),
                    new BField(String, CPDate),
                    new BField(String, Case)
                })
            };

            ListFormSearchFields = new[]
            {
                new BField(String, CardNumber),
                new BField(String, TransactionCode)
                {
                    ComponentType = ComponentType.BParameterComponent
                },
                new BField(String, UsageCode)
                {
                    ComponentType = ComponentType.BParameterComponent
                },
                new BField(String, CardType)
                {
                    ComponentType = ComponentType.BParameterComponent
                },
                new BField(String, Validation),
                new BField(String, ClearingStatus)
                {
                    ComponentType = ComponentType.BParameterComponent
                },
                new BField(DateTime, ClearingDateBegin),
                new BField(DateTime, ClearingDateEnd)
            };
        }
        #endregion
    }
}