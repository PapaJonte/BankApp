using System;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transaction_Layer.Controller;

public class BankTransaction(StageTransaction stageTransaction)
{

    public void RequestTransaction(string originAccountNumber, string targetAccountNumber, decimal depositAmount,decimal withdrawalAmount)
    {
        RequestDeposit(targetAccountNumber, withdrawalAmount);
        RequestWithdrawal(originAccountNumber, depositAmount);
    }

    public void RequestDeposit(string accountNumber, decimal amount)
    {
        stageTransaction.Stage(accountNumber, amount, TransactionType.Deposit);
    }
    
    public void RequestWithdrawal(string accountNumber, decimal amount)
    {
        stageTransaction.Stage(accountNumber, amount, TransactionType.Withdrawal);
    }
}
