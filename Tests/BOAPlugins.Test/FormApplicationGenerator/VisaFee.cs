using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            IsTabForm = true;

            Tabs = new[]
            {
                new BTab(FieldName.Amounts.ToString(), new[]
                {
                    new BField(DateTime, FieldName.TransactionDate),
                    new BField(Decimal, FieldName.FeeAmount),
                    new BField(Int32, FieldName.FeeAmountCurrency),
                    new BField(Decimal, FieldName.SourceAmount),
                    new BField(Int32, FieldName.SourceAmountCurrency)
                }),

                new BTab(FieldName.GeneralInformation.ToString(), new[]
                {
                    new BField(String, FieldName.TransactionCode)
                    {
                        ComponentType = ComponentType.BParameterComponent
                    },
                    new BField(String, FieldName.UsageCode)
                    {
                        ComponentType = ComponentType.BParameterComponent
                    },
                    new BField(String, FieldName.CardTye)
                    {
                        ComponentType = ComponentType.BParameterComponent
                    },
                    new BField(String, FieldName.SourceBIN),
                    new BField(String, FieldName.DestinationBIN),
                    new BField(String, FieldName.Direction),
                    new BField(String, FieldName.CardNumber),
                    new BField(DateTime, FieldName.TranDate),
                    new BField(DateTime, FieldName.ClearingDate),
                    new BField(String, FieldName.ClearingStatus)
                }),

                new BTab(FieldName.ReasonInformation.ToString(), new[]
                {
                    new BField(Int32, FieldName.ReasonCode),
                    new BField(Int32, FieldName.CountrCode),
                    new BField(DateTime, FieldName.Message)
                }),

                new BTab(FieldName.Status.ToString(), new[]
                {
                    new BField(Boolean, FieldName.Incoming),
                    new BField(Boolean, FieldName.Outgoing),
                    new BField(String, FieldName.SettlementFlag),
                    new BField(String, FieldName.Reimbursement),
                    new BField(String, FieldName.EndOfDay),
                    new BField(String, FieldName.Validation),
                    new BField(String, FieldName.ValidationDefinition),
                    new BField(String, FieldName.CPDate),
                    new BField(String, FieldName.Case)
                })
            };
        }
        #endregion
    }

    [TestClass]
    public class TestGenerate
    {
        #region Public Methods
        [TestMethod]
        public void T1()
        {
            new VisaFee().AutoGenerateCodesAndExportFiles();
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
        ClearingDateEnd,

        Status
    }
}