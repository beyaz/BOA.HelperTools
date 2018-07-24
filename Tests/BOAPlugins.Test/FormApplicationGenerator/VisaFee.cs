using System.Collections.Generic;

namespace BOAPlugins.FormApplicationGenerator
{
    static class SolutionFile
    {
        #region Constants
        public const string CardPaymentSystem_Clearing = @"D:\work\BOA.BusinessModules\Dev\BOA.CardPaymentSystem.Clearing\BOA.CardPaymentSystem.Clearing.sln";
        #endregion
    }

    class GroupBoxInfo
    {
        public GroupBoxInfo(string title, IReadOnlyCollection<FieldInfo> fields)
        {
            Fields = fields;
            Title = title;
        }

        public IReadOnlyCollection<FieldInfo> Fields { get; set; }
        public string Title { get; set; }
    }



    public class VisaFee : Model
    {
        IReadOnlyCollection<GroupBoxInfo> GroupBoxes = new []
        {
            new GroupBoxInfo(FieldName.Amounts.ToString(),new []
            {
                new FieldInfo(FieldName.TransactionDate, DotNetTypeName.DateTime),
                new FieldInfo(FieldName.FeeAmount, DotNetTypeName.Decimal),
                new FieldInfo(FieldName.FeeAmountCurrency, DotNetTypeName.Int32),
                new FieldInfo(FieldName.SourceAmount, DotNetTypeName.Decimal),
                new FieldInfo(FieldName.SourceAmountCurrency, DotNetTypeName.Int32),
            }),

            new GroupBoxInfo(FieldName.GeneralInformation.ToString(),new []
            {
                new FieldInfo(FieldName.TransactionCode, DotNetTypeName.String)
                {
                    ComponentName =  ComponentName.BParameterComponent
                },
                new FieldInfo(FieldName.UsageCode, DotNetTypeName.String)
                {
                    ComponentName =  ComponentName.BParameterComponent
                },
                new FieldInfo(FieldName.CardTye, DotNetTypeName.String)
                {
                    ComponentName =  ComponentName.BParameterComponent
                },
                new FieldInfo(FieldName.SourceBIN, DotNetTypeName.String),
                new FieldInfo(FieldName.DestinationBIN, DotNetTypeName.String),
                new FieldInfo(FieldName.Direction, DotNetTypeName.String),
                new FieldInfo(FieldName.CardNumber, DotNetTypeName.String),
                new FieldInfo(FieldName.TranDate, DotNetTypeName.DateTime),
                new FieldInfo(FieldName.ClearingDate, DotNetTypeName.DateTime),
                new FieldInfo(FieldName.ClearingStatus, DotNetTypeName.String),
            }),

            new GroupBoxInfo(FieldName.ReasonInformation.ToString(),new []
            {
                new FieldInfo(FieldName.ReasonCode, DotNetTypeName.Int32),
                new FieldInfo(FieldName.CountrCode, DotNetTypeName.Int32),
                new FieldInfo(FieldName.Message, DotNetTypeName.DateTime)
            }),
        };

        #region Constructors
        public VisaFee() : base(SolutionFile.CardPaymentSystem_Clearing, "VisaFee")
        {
            
            
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