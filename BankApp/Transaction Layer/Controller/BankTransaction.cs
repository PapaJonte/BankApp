using System;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transaction_Layer.Controller;

public class BankTransaction(StageTransaction stageTransaction)
{

    /// <summary>
    /// Responsible for requesting a deposit of money into an account.
    /// Stages a deposit transaction at the transaction module.
    /// </summary>
    /// <param name="">
    /// string accountNumber, number of the account we deposit money to.
    /// decimal amount, value we add to the bank account.
    /// </param>
    public void RequestDeposit(string accountNumber, decimal amount)
    {
        stageTransaction.Stage(accountNumber, amount, TransactionType.Deposit);
    }
    
    /// <summary>
    /// Responsible for requesting a withdrawal of money from an account.
    /// Stages a withdrawal transaction at the transaction module.
    /// </summary>
    /// <param name="">
    /// string accountNumber, number of the account we withdraw money from.
    /// decimal amount, value we remove from the bank account.
    /// </param>
    public void RequestWithdrawal(string accountNumber, decimal amount)
    {
        stageTransaction.Stage(accountNumber, amount, TransactionType.Withdrawal);
    }
}

