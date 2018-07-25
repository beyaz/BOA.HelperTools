namespace BOAPlugins.FormApplicationGenerator
{
    public class Outgoing : Model
    {
        #region Constructors
        public Outgoing() : base(SolutionFile.CardPaymentSystem_Clearing, nameof(Outgoing))
        {

            Cards = new[]
            {
                new BCard(FieldName.Amounts.ToString(), new[]
                {
                    new BField(DateTime, FieldName.TransactionDate),
                    new BField(Decimal, FieldName.FeeAmount),
                    new BField(Int32, FieldName.FeeAmountCurrency),
                    new BField(Decimal, FieldName.SourceAmount),
                    new BField(Int32, FieldName.SourceAmountCurrency)
                }),

                new BCard(FieldName.GeneralInformation.ToString(), new[]
                {
                    new BField(String, FieldName.TransactionCode)
                    {
                        ComponentType = ComponentType.BParameterComponent
                    },
                    new BField(String, FieldName.UsageCode)
                    {
                        ComponentType = ComponentType.BParameterComponent
                    },
                    new BField(String, FieldName.CardType)
                    {
                        ComponentType = ComponentType.BParameterComponent
                    },
                    new BField(String, FieldName.SourceBIN),
                    new BField(String, FieldName.DestinationBIN),
                    new BField(String, FieldName.Direction),
                    new BField(String, FieldName.CardNumber),

                    new BField(DateTime, FieldName.ClearingDate),
                    new BField(String, FieldName.ClearingStatus)
                    {
                        ComponentType = ComponentType.BParameterComponent
                    }
                }),

                
            };



            ListFormSearchFields = new[]
            {
                new BField(String, FieldName.CardNumber),
                new BField(String, FieldName.TransactionCode)
                {
                    ComponentType = ComponentType.BParameterComponent
                },
                new BField(String, FieldName.UsageCode)
                {
                    ComponentType = ComponentType.BParameterComponent
                },
                new BField(String, FieldName.CardType)
                {
                    ComponentType = ComponentType.BParameterComponent
                },
                new BField(String, FieldName.Validation),
                new BField(String, FieldName.ClearingStatus)
                {
                    ComponentType = ComponentType.BParameterComponent
                },
                new BField(DateTime, FieldName.ClearingDateBegin),
                new BField(DateTime, FieldName.ClearingDateEnd)
            };
        }
        #endregion
    }
}