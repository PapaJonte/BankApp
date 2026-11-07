using System;
using BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transaction_Layer.Service;

namespace BankApp.Transaction_Layer.API;

public class BankTransferStatistic(TransactionStatisticFilter transactionStatisticFilter)
{
    #region user stats
    public AccountPopularityVolume AccountPopularityTotalVolume(int userID)
    {
        StatusCode status = transactionStatisticFilter.TryOrderByTotalVolume(userID, out IReadOnlyList<TotalVolume> orderedAccountList);

        AccountPopularityVolume accPopVol = new(userID, orderedAccountList) { StatusCode = status };

        return accPopVol;
    }

    public AccountPopularityCount AccountPopularityCount(int userID)
    {
        StatusCode status = transactionStatisticFilter.TryOrderByTransactionCount(userID, out IReadOnlyList<TotalCount> orderedAccountList);

        AccountPopularityCount accPopCount = new(userID, orderedAccountList) { StatusCode = status };

        return accPopCount;
    }

    public TotalTransactionAmount FullTransactionAmount(int userID)
    {
        StatusCode status = transactionStatisticFilter.TryGetFullTransactionAmount(userID, out decimal totalAmount);

        TotalTransactionAmount totalTransactionAmount = new(userID, totalAmount) { StatusCode = status };

        return totalTransactionAmount;
    }

    public TotalTransactionsDateOrder TransactionsByDate(int userID)
    {
        StatusCode status = transactionStatisticFilter.TryGetTransactionsByDate(userID, out IReadOnlyDictionary<int, List<TransactionDate>> orderedTransactionDict);

        Dictionary<int, List<TransactionDate>> transactionDates = [];

        TotalTransactionsDateOrder dateOrder = new(userID, orderedTransactionDict) { StatusCode = status };

        return dateOrder;
    }
    #endregion

    #region admin stats

    public TotalTransactions GetTotalBankTransactions()
    {

    }
    public MostPopularAccounts GetMostPopularAccounts()
    {
        
    }
    #endregion
}
