namespace BOAPlugins.FormApplicationGenerator
{
    static class SolutionFile
    {
        #region Constants
        public const string CardPaymentSystem_Clearing = @"D:\work\BOA.BusinessModules\Dev\BOA.CardPaymentSystem.Clearing\BOA.CardPaymentSystem.Clearing.sln";
        #endregion
    }

    public class VisaFee : Model
    {
        #region Constructors
        public VisaFee() : base(SolutionFile.CardPaymentSystem_Clearing, nameof(VisaFee))
        {
            Cards = new[]
            {
                new BCard(FieldName.Amounts.ToString(), new[]
                {
                    new BField(FieldName.TransactionDate, DateTime),
                    new BField(FieldName.FeeAmount, Decimal),
                    new BField(FieldName.FeeAmountCurrency, Int32),
                    new BField(FieldName.SourceAmount, Decimal),
                    new BField(FieldName.SourceAmountCurrency, Int32)
                }),

                new BCard(FieldName.GeneralInformation.ToString(), new[]
                {
                    new BField(FieldName.TransactionCode, String)
                    {
                        ComponentType = ComponentType.BParameterComponent
                    },
                    new BField(FieldName.UsageCode, String)
                    {
                        ComponentType = ComponentType.BParameterComponent
                    },
                    new BField(FieldName.CardTye, String)
                    {
                        ComponentType = ComponentType.BParameterComponent
                    },
                    new BField(FieldName.SourceBIN, String),
                    new BField(FieldName.DestinationBIN, String),
                    new BField(FieldName.Direction, String),
                    new BField(FieldName.CardNumber, String),
                    new BField(FieldName.TranDate, DateTime),
                    new BField(FieldName.ClearingDate, DateTime),
                    new BField(FieldName.ClearingStatus, String)
                }),

                new BCard(FieldName.ReasonInformation.ToString(), new[]
                {
                    new BField(FieldName.ReasonCode, Int32),
                    new BField(FieldName.CountrCode, Int32),
                    new BField(FieldName.Message, DateTime)
                })
            };
        }
        #endregion
    }

    public enum FieldName
    {
        ReasonInformation,
        GeneralInformation,
        TransactionDate,
        Amounts,
        FeeAmount,
        FeeAmountCurrency,
        SourceAmount,
        SourceAmountCurrency,
        TransactionCode,
        UsageCode,
        CardTye,
        SourceBIN,
        DestinationBIN,
        Direction,
        CardNumber,
        TranDate,
        ClearingDate,
        ClearingStatus,
        ReasonCode,
        CountrCode,
        Message,
        Incoming,
        Outgoing,
        SettlementFlag,
        Reimbursement,
        EndOfDay,
        Validation,
        ValidationDefinition,
        CPDate,
        Case,
        ClearingDateBegin,
        ClearingDateEnd
    }
}