using System;
using BankApp.Transaction_Layer.API;
using BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;
using BankApp.Transaction_Layer.Interface;

namespace BankApp.Transaction_Layer.Controller;

/// <summary>
/// Bridge class between front end and transfer layer
/// </summary>
/// <param name="bankTransferStatistic"></param>
public class TransactionStatisticBridge(BankTransferStatistic bankTransferStatistic) : IViewTransactionStatisticBridge
{
    /// <summary>
    /// Fetches filtered object based on account popularity on amount of saving accounts.
    /// </summary>
    /// <param name="userID"></param>
    /// <returns></returns>
    public AccountPopularityCount GetAccountPopularityCount(int userID) => bankTransferStatistic.AccountPopularityCount(userID);
    /// <summary>
    /// Fetches filtered object based on account popularity on sum of each account assets.
    /// </summary>
    /// <param name="userID"></param>
    /// <returns></returns>
    public AccountPopularityVolume GetAccountPopularityVolume(int userID) => bankTransferStatistic.AccountPopularityTotalVolume(userID);
    /// <summary>
    /// Fetches filtered object based on total transactions for userID.
    /// </summary>
    /// <param name="userID"></param>
    /// <returns></returns>
    public TotalTransactionAmount GetTotalTransactionAmount(int userID) => bankTransferStatistic.FullTransactionAmount(userID);
    /// <summary>
    /// Fetches filtered object based on total transactions date time.
    /// </summary>
    /// <param name="userID"></param>
    /// <returns></returns>
    public TotalTransactionsDateOrder GetTotalTransactionsDateOrder(int userID) => bankTransferStatistic.TransactionsByDate(userID);
}
