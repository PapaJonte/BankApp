using System;
using BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transaction_Layer.Service;
using Spectre.Console;

namespace BankApp.Transaction_Layer.API;

public class BankTransferStatistic(TransactionStatisticFilter transactionStatisticFilter)
{
    #region user stats
    /// <summary>
    /// Fetches and returns statistics over account popularity by total volume. Volume meaning sum of monetary asset.
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public AccountPopularityVolume AccountPopularityTotalVolume(int userID)
    {
        StatusCode status = transactionStatisticFilter.TryOrderByTotalVolume(userID, out IReadOnlyList<TotalVolume> orderedAccountList);

        AccountPopularityVolume accPopVol = new(userID, orderedAccountList) { StatusCode = status };

        return accPopVol;
    }

    /// <summary>
    /// Fetches and returns statistic object over account popularity by count. Count meaning how many transactions in the account.
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public AccountPopularityCount AccountPopularityCount(int userID)
    {
        StatusCode status = transactionStatisticFilter.TryOrderByTransactionCount(userID, out IReadOnlyList<TotalCount> orderedAccountList);

        AccountPopularityCount accPopCount = new(userID, orderedAccountList) { StatusCode = status };

        return accPopCount;
    }

    /// <summary>
    /// Returns statistic object containing sum of transactions
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public TotalTransactionAmount FullTransactionAmount(int userID)
    {
        StatusCode status = transactionStatisticFilter.TryGetFullTransactionAmount(userID, out decimal totalAmount);

        TotalTransactionAmount totalTransactionAmount = new(userID, totalAmount) { StatusCode = status };

        return totalTransactionAmount;
    }
    
    /// <summary>
    /// Returns statistic object containing over transactions by date
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public TotalTransactionsDateOrder TransactionsByDate(int userID)
    {
        StatusCode status = transactionStatisticFilter.TryGetTransactionsByDate(userID, out IReadOnlyDictionary<int, List<TransactionDate>> orderedTransactionDict);

        Dictionary<int, List<TransactionDate>> transactionDates = [];

        TotalTransactionsDateOrder dateOrder = new(userID, orderedTransactionDict) { StatusCode = status };

        return dateOrder;
    }
    #endregion

    #region admin stats
    /// <summary>
    /// Get total transaction value add it to wrapper object and return
    /// </summary>
    /// <returns></returns>
    public TotalTransactions GetTotalBankTransactions()
    {
        StatusCode status = transactionStatisticFilter.TryGetFullTransactionAmount(out decimal total);

        TotalTransactions totalTransactions = new(total) { StatusCode = status };
        return totalTransactions;
    }

    /// <summary>
    /// get ordered list of most popular accounts based on transactions and add it to wrapper object and returns.
    /// </summary>
    /// <returns></returns>
    public MostPopularAccounts GetMostPopularAccounts()
    {
        StatusCode status = transactionStatisticFilter.TryGetPopularAccounts(out IReadOnlyList<TotalCount>? orderedAccountList);

        MostPopularAccounts mostPopularAccounts = new(orderedAccountList) { StatusCode = status };

        return mostPopularAccounts;
    }
    #endregion
}
