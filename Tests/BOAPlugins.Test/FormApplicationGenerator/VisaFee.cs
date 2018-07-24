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
                    new BField(FieldName.TransactionDate, DotNetType.DateTime),
                    new BField(FieldName.FeeAmount, DotNetType.Decimal),
                    new BField(FieldName.FeeAmountCurrency, DotNetType.Int32),
                    new BField(FieldName.SourceAmount, DotNetType.Decimal),
                    new BField(FieldName.SourceAmountCurrency, DotNetType.Int32)
                }),

                new BCard(FieldName.GeneralInformation.ToString(), new[]
                {
                    new BField(FieldName.TransactionCode, DotNetType.String)
                    {
                        ComponentType = ComponentType.BParameterComponent
                    },
                    new BField(FieldName.UsageCode, DotNetType.String)
                    {
                        ComponentType = ComponentType.BParameterComponent
                    },
                    new BField(FieldName.CardTye, DotNetType.String)
                    {
                        ComponentType = ComponentType.BParameterComponent
                    },
                    new BField(FieldName.SourceBIN, DotNetType.String),
                    new BField(FieldName.DestinationBIN, DotNetType.String),
                    new BField(FieldName.Direction, DotNetType.String),
                    new BField(FieldName.CardNumber, DotNetType.String),
                    new BField(FieldName.TranDate, DotNetType.DateTime),
                    new BField(FieldName.ClearingDate, DotNetType.DateTime),
                    new BField(FieldName.ClearingStatus, DotNetType.String)
                }),

                new BCard(FieldName.ReasonInformation.ToString(), new[]
                {
                    new BField(FieldName.ReasonCode, DotNetType.Int32),
                    new BField(FieldName.CountrCode, DotNetType.Int32),
                    new BField(FieldName.Message, DotNetType.DateTime)
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