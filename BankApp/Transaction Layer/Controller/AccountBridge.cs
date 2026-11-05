
using BankApp.GustafsClasses;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transaction_Layer.Interface;
using Org.BouncyCastle.Asn1.Cms;

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
        var accs = GetAccounts(userID, accountNumber);
        if (accs.Item1.GetThresholdNotification(amount, TransactionType.Deposit) >= amount)
        {
            //call notification service when impl
        }
        accs.Item2.TransferBalance(amount, true);
    }

    public void Withdraw(int userID, int accountNumber, decimal amount)
    {
        var accs = GetAccounts(userID, accountNumber);
        if (accs.Item1.GetThresholdNotification(amount, TransactionType.Withdrawal) >= amount)
        {
            //call notification service when impl
        }
        accs.Item2.TransferBalance(amount, false);
    }

    private SavingsAccount GetSavingsAccount(int userID, int accountNumber)
    {
        var bankAccounts = accountManager.BankAccountDictionary[userID] ?? throw new NullReferenceException($"No accounts connected to user ID {userID}");

        var savingsAccounts = bankAccounts.FirstOrDefault(x => x.GetUserId() == userID).SavingAccounts ?? throw new NullReferenceException($"No savings accounts connected to user ID {userID}");

        var savingsAccount = savingsAccounts.FirstOrDefault(x => x.GetSavingsAccountNumber() == accountNumber) ?? throw new NullReferenceException($"No savings account connected to account number {accountNumber}");

        return savingsAccount;
    }

    private (BankAccount, SavingsAccount) GetAccounts(int userID, int accountNumber)
    {
        BankAccount? bankAccount = null;
        SavingsAccount savingsAccount;
        var bankAccounts = accountManager.BankAccountDictionary[userID] ?? throw new NullReferenceException($"No accounts connected to user ID {userID}");

        foreach (var b in bankAccounts)
        {
            foreach (var s in b.SavingAccounts)
            {
                if (s.GetSavingsAccountNumber() == accountNumber)
                {
                    bankAccount = b;
                    break;
                }
            }
        }

        if (bankAccount == null) throw new NullReferenceException($"Could not find bank account for user {userID} and account number{accountNumber}");

        savingsAccount = bankAccount.SavingAccounts.FirstOrDefault(x => x.GetSavingsAccountNumber() == accountNumber) ?? throw new NullReferenceException($"No savings account connected to account number {accountNumber}");

        (BankAccount, SavingsAccount) bns = new(bankAccount, savingsAccount);

        return bns;
    }

}
