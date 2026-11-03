using System;
using BankApp.Transaction_Layer.API;
using BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;
using BankApp.Transaction_Layer.Interface;

namespace BankApp.Transaction_Layer.Controller;

public class TransactionStatisticBridge(BankTransferStatistic bankTransferStatistic) : IViewTransactionStatisticBridge
{
    public AccountPopularityCount GetAccountPopularityCount(int userID) => bankTransferStatistic.AccountPopularityCount(userID);

    public AccountPopularityVolume GetAccountPopularityVolume(int userID) => bankTransferStatistic.AccountPopularityTotalVolume(userID);

    public TotalTransactionAmount GetTotalTransactionAmount(int userID) => bankTransferStatistic.FullTransactionAmount(userID);

    public TotalTransactionsDateOrder GetTotalTransactionsDateOrder(int userID) => bankTransferStatistic.TransactionsByDate(userID);
}
