using System;
using BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;

namespace BankApp.Transaction_Layer.Interface;

public interface IViewTransactionStatisticBridge
{
    public AccountPopularityCount GetAccountPopularityCount(int userID);
    public AccountPopularityVolume GetAccountPopularityVolume(int userID);
    public TotalTransactionAmount GetTotalTransactionAmount(int userID);
    public TotalTransactionsDateOrder GetTotalTransactionsDateOrder(int userID);
}
