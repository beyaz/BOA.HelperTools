
USE BOA
GO

DECLARE @batchSize INT = 100000 -- id alaný ardýþýk olmadýðý için ortalama her sefernce 5 bin verilince 250 satýr atýyor , 100 bin verilince de ortalama 5biner insert yapýyor. 100 bin verilince toplamda 3 dk da çalýþýyor
DECLARE @minId     INT
DECLARE @maxId     INT

TRUNCATE table  DBT.DebitTransaction

SET @minId = (select MIN(id) FROM dbkuveyt2.dbo.ATMislemleri With(nolock))
SET @maxId = @minId + @batchSize

WHILE 1 = 1
BEGIN

	INSERT INTO DBT.DebitTransaction
		(
			BusinessKey,
			ExternalTransactionType,
			ExternalTransactionCode,
			InternalResponseCode,
			ExternalResponseCode,
			ReferenceNumber,
			TerminalId,
			TerminalBranchId,
			TransactionCode,
			ChannelId,
			IsReversed,
			AccountNumber,
			AccountSuffix,
			TransactionDate,
			ReverseTransactionDate,
			CardNumber,
			FromBranchId,
			FromAccountNumber,
			FromAccountSuffix,
			ToAccountNumber,
			ToAccountSuffix,
			ToBranchId,
			ProcessTransactionDate,
			ProcessTransactionTime,
			AuthorizationCode,
			AcquirerId,
			BillingAmount,
			BillingCurrencyCode,
			SettlementAmount,
			SettlementCurrencyCode,
			OriginalAmount,
			OriginalCurrencyCode,
			CommissionAmount,
			CashbackAmount,
			CashbackTaxRate,
			BlockageId,
			TaxNumber,
			IsMernisChecked,
			CardBrand,
			MerchantNumber,
			Location,
			AmountUsedFromKMH,
			AmountUsedFromAccount,
			KMHPackageId,
			KMHAccountSuffix,
			CurrentAmountScore,
			KMHScore,
			ErrorMessage,
			TableId,
			TranBranchId,
			TransactionReference,
			UserName,
			HostName,
			SystemDate
		)
    
		SELECT 
			NULL AS BusinessKey,
			t.ATMislemlerid AS ExternalTransactionType,
			t.TransactionCode AS ExternalTransactionCode,
			NULL AS InternalResponseCode,
			t.ATMresponseid AS ExternalResponseCode,
			t.RetrivalReference AS ReferenceNumber,
			t.Termid AS TerminalId,
			NULL AS TerminalBranchId,
			t.TransactionCode AS TransactionCode,
			0 AS ChannelId,
			t.iptalMi AS IsReversed,
			t.musterino AS AccountNumber,
			t.ekno AS AccountSuffix,
			t.islemTarihi AS TransactionDate,
			NULL AS ReverseTransactionDate,
			t.CardNumber AS CardNumber,
			t.BranchFrom AS FromBranchId,
			CRD.fSplitStringGetValueAt(t.AccNumberFrom , '-' , 0)  AS FromAccountNumber,
			CRD.fSplitStringGetValueAt(t.AccNumberFrom , '-' , 1)  AS FromAccountSuffix,
			CRD.fSplitStringGetValueAt(t.AccNumberTo , '-' , 0) AS ToAccountNumber, 
			CRD.fSplitStringGetValueAt(t.AccNumberTo , '-' , 1) AS ToAccountSuffix,
			t.BranchTo AS ToBranchId,
			t.TransactionDate AS ProcessTransactionDate,
			t.TransactionTime AS ProcessTransactionTime,
			t.authorizasyonKodu AS AuthorizationCode,
			t.acquirerid AS AcquirerId,
			t.BillingAmount AS BillingAmount,
			t.BillingCurrency AS BillingCurrency,
			t.SettlementAmount AS SettlementAmount,
			t.SettlementCurrency AS SettlementCurrency,
			t.TransactionAmount AS OriginalAmount,
			t.TransactionCurrency AS OriginalCurrency,
			t.komisyon AS CommissionAmount,
			NULL AS CashbackAmount,
			NULL AS CashbackTaxRate,
			b.id AS BlockageId,
			t.Tckn AS TaxNumber,
			t.IsMernisChecked AS IsMernisChecked,
			NULL AS CardBrand,
			NULL AS MerchantNumber,
			gs.Location AS Location,
			gs.KMHAmount AS AmountUsedFromKMH,
			gs.CurrentAmount AS AmountUsedFromAccount,
			gs.KMHPackageId AS KMHPackageId,
			gs.CreditAccountSuffix AS KMHAccountSuffix,
			gs.CurrentScore AS CurrentAmountScore,
			gs.KMHScore AS KMHScore,
			NULL AS ErrorMessage,
			NULL AS TableId,
			t.islemSube AS TranBranchId,
			t.islemReferansi AS TransactionReference,
			NULL AS UserName,
			NULL AS HostName,
			NULL AS SystemDate
	
		FROM dbkuveyt2.dbo.ATMislemleri AS t  WITH(NOLOCK) LEFT JOIN
			 BOA.DBT.DebitCardGoldScore AS gs WITH(NOLOCK) ON t.RetrivalReference = gs.AggreementNo LEFT JOIN
			 dbkuveyt2.dbo.banharblokaj AS b  WITH(NOLOCK) ON b.id = t.blokeid
	   WHERE t.id >= @minId AND t.id < @maxId AND
	         t.TransactionCode IN (1110,1103,1203,1303,1007,1107,1207,1307)
			 

	 IF @@ROWCOUNT = 0 BREAK

	 SET @minId = @minId + @batchSize
	 SET @maxId = @maxId + @batchSize

END


