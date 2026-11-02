using System;
using BankApp.Transaction_Layer.Interface;

namespace BankApp.Transaction_Layer.Controller;

internal class AccountBridge(AccountManager accountManager) : IAccountBridge
{

    public bool CanWithdraw(int userID, int accountNumber, decimal amount)
    {
        var savingsAccount = GetSavingsAccount(userID, accountNumber);
        return savingsAccount.GetBalance() >= amount;
    }

    public void Deposit(int userID, int accountNumber, decimal amount)
    {
        var savingsAccount = GetSavingsAccount(userID, accountNumber);
        savingsAccount.TransferBalance(amount, true);
    }

    public void Withdraw(int userID, int accountNumber, decimal amount)
    {
        var savingAccount = GetSavingsAccount(userID, accountNumber);
        savingAccount.TransferBalance(amount,false);
    }

    private SavingsAccount GetSavingsAccount(int userID, int accountNumber)
    {
        var bankAccounts = accountManager.BankAccountDictionary[userID] ?? throw new NullReferenceException($"No accounts connected to user ID {userID}");

        var savingsAccounts = bankAccounts.FirstOrDefault(x => x.GetUserId() == userID).savingAccounts ?? throw new NullReferenceException($"No savings accounts connected to user ID {userID}");

        var savingsAccount = savingsAccounts.FirstOrDefault(x => x.GetSavingsAccountNumber() == accountNumber) ?? throw new NullReferenceException($"No savings account connected to account number {accountNumber}");

        return savingsAccount;
    }
}
