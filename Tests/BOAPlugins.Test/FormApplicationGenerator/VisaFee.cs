using System.Collections.Generic;

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
        public VisaFee() : base(SolutionFile.CardPaymentSystem_Clearing, "VisaFee")
        {
            FormDataClassFields = new List<FieldInfo>
            {
                new FieldInfo(FieldName.TransactionDate, DotNetTypeName.DateTime, FieldName.Amounts),
                new FieldInfo(FieldName.FeeAmount, DotNetTypeName.Decimal, FieldName.Amounts),
                new FieldInfo(FieldName.FeeAmountCurrency, DotNetTypeName.Int32, FieldName.Amounts),
                new FieldInfo(FieldName.SourceAmount, DotNetTypeName.Decimal, FieldName.Amounts),
                new FieldInfo(FieldName.SourceAmountCurrency, DotNetTypeName.Int32, FieldName.Amounts),

                new FieldInfo(FieldName.TransactionCode, DotNetTypeName.String, FieldName.GeneralInformation)
                {
                    ComponentName =  ComponentName.BParameterComponent
                },
                new FieldInfo(FieldName.UsageCode, DotNetTypeName.String, FieldName.GeneralInformation)
                {
                    ComponentName =  ComponentName.BParameterComponent
                },
                new FieldInfo(FieldName.CardTye, DotNetTypeName.String, FieldName.GeneralInformation)
                {
                    ComponentName =  ComponentName.BParameterComponent
                },
                new FieldInfo(FieldName.SourceBIN, DotNetTypeName.String, FieldName.GeneralInformation),
                new FieldInfo(FieldName.DestinationBIN, DotNetTypeName.String, FieldName.GeneralInformation),
                new FieldInfo(FieldName.Direction, DotNetTypeName.String, FieldName.GeneralInformation),
                new FieldInfo(FieldName.CardNumber, DotNetTypeName.String, FieldName.GeneralInformation),
                new FieldInfo(FieldName.TranDate, DotNetTypeName.DateTime, FieldName.GeneralInformation),
                new FieldInfo(FieldName.ClearingDate, DotNetTypeName.DateTime, FieldName.GeneralInformation),
                new FieldInfo(FieldName.ClearingStatus, DotNetTypeName.String, FieldName.GeneralInformation),

                new FieldInfo(FieldName.ReasonCode, DotNetTypeName.Int32, FieldName.ReasonInformation),
                new FieldInfo(FieldName.CountrCode, DotNetTypeName.Int32, FieldName.ReasonInformation),
                new FieldInfo(FieldName.Message, DotNetTypeName.DateTime, FieldName.ReasonInformation),

                new FieldInfo(FieldName.TransactionDate, DotNetTypeName.DateTime, FieldName.Amounts),
                new FieldInfo(FieldName.TransactionDate, DotNetTypeName.DateTime, FieldName.Amounts),
                new FieldInfo(FieldName.TransactionDate, DotNetTypeName.DateTime, FieldName.Amounts),
                new FieldInfo(FieldName.TransactionDate, DotNetTypeName.DateTime, FieldName.Amounts),
                new FieldInfo(FieldName.TransactionDate, DotNetTypeName.DateTime, FieldName.Amounts),
                new FieldInfo(FieldName.TransactionDate, DotNetTypeName.DateTime, FieldName.Amounts),
                new FieldInfo(FieldName.TransactionDate, DotNetTypeName.DateTime, FieldName.Amounts),
                new FieldInfo(FieldName.TransactionDate, DotNetTypeName.DateTime, FieldName.Amounts),
                new FieldInfo(FieldName.TransactionDate, DotNetTypeName.DateTime, FieldName.Amounts),
                new FieldInfo(FieldName.TransactionDate, DotNetTypeName.DateTime, FieldName.Amounts)
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