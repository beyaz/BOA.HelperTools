using static BOAPlugins.FormApplicationGenerator.FieldName;

namespace BOAPlugins.FormApplicationGenerator
{
    public class Outgoing : Model
    {
        #region Constructors
        public Outgoing() : base(SolutionFile.CardPaymentSystem_Clearing, nameof(Outgoing))
        {
            Cards = new[]
            {
                new BCard(Draft.ToString(), new[]
                {
                    new BField(Decimal, DraftSelling),
                    new BField(Decimal, DraftWithdrawal),
                    new BField(Decimal, DraftRefund)
                }),

                new BCard(Chargeback.ToString(), new[]
                {
                    new BField(Decimal, ChargebackSelling),
                    new BField(Decimal, ChargebackWithdrawal),
                    new BField(Decimal, ChargebackRefund)
                }),

                new BCard(Fee + "" + ForwardSlash + Fund, new[]
                {
                    new BField(Decimal, FeeCollect),
                    new BField(Decimal, FundDistribution)
                }),

                new BCard(Fraud + "" + ForwardSlash + Text, new[]
                {
                    new BField(String, Fraud),
                    new BField(String, TextData)
                }),

                new BCard(Reversal.ToString(), new[]
                {
                    new BField(Decimal, ReversalSelling),
                    new BField(Decimal, ReversalWithdrawal),
                    new BField(Decimal, ReversalRefund)
                }),
                new BCard(ChargebackReversal.ToString(), new[]
                {
                    new BField(Decimal, ChargebackReversalSelling),
                    new BField(Decimal, ChargebackReversalWithdrawal),
                    new BField(Decimal, ChargebackReversalRefund)
                }),

                new BCard(Document.ToString(), new[]
                {
                    new BField(String, OriginalRequest),
                    new BField(String, CopyRequest),
                    new BField(String, Confirm)
                }),

                new BCard(BatchFileTrailer + "", new[]
                {
                    new BField(String, FileHeader),
                    new BField(String, BatchTrailer),
                    new BField(String, FileTrailer)
                })
            };

            ListFormSearchFields = new[]
            {
                new BField(String, CardNumber),
                new BField(DateTime, ClearingDateBegin),
                new BField(DateTime, ClearingDateEnd),
                new BField(String, TransactionCode),
                new BField(Boolean, ToBeSent)
            };
        }
        #endregion
    }
}