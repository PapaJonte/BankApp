using System;
using BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;
using BankApp.Transactions.Statistics.Models;

namespace BankApp.Transactions.Statistics;

public interface IViewTransactionStatisticBridge
{
    public AccountPopularityCount GetAccountPopularityCount(int userID);
    public AccountPopularityVolume GetAccountPopularityVolume(int userID);
    public TotalTransactionAmount GetTotalTransactionAmount(int userID);
    public TotalTransactionsDateOrder GetTotalTransactionsDateOrder(int userID);
}
