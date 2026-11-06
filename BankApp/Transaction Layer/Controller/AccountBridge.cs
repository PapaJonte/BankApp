
using System.Xml.Linq;
using BankApp.GustafsClasses;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transaction_Layer.Interface;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Crypto.Modes;

namespace BankApp.Transaction_Layer.Controller;

internal class AccountBridge(AccountManager accountManager) : IAccountBridge
{

    public bool CanWithdraw(int userID, int accountNumber, decimal amount)
    {
        if (TryGetBankAccount(userID, accountNumber, out var bankAccount))
        {
            return bankAccount!.GetBalance() >= amount;
        }
        else if (TryGetSavingsAccount(userID, accountNumber, out var savingsAccount))
        {
            return savingsAccount!.GetBalance() >= amount;
        }
        return false;
    }

    public void Deposit(int userID, int accountNumber, decimal amount)
    {
        if (TryGetSavingsAccount(userID, accountNumber, out var savingsAccount))
        {
            if (TryGetBankAccount(userID, accountNumber, out var bankAccount))
            {
                if (bankAccount!.GetThresholdNotification(amount, TransactionType.Deposit) <= amount)
                {
                    //call notification service when impl
                }
                savingsAccount!.TransferBalance(amount, true);
            }
        }
        else if (TryGetBankAccount(userID, accountNumber, out var bankAccount))
        {
            if (bankAccount!.GetThresholdNotification(amount, TransactionType.Deposit) <= amount)
            {
                //call notification service when impl
            }
            bankAccount.TransferBalance(amount, true);
        }
    }

    public void Withdraw(int userID, int accountNumber, decimal amount)
    {
        if (TryGetSavingsAccount(userID, accountNumber, out var savingsAccount))
        {
            if (TryGetBankAccount(userID, accountNumber, out var bankAccount))
            {
                if (bankAccount!.GetThresholdNotification(amount, TransactionType.Withdrawal) <= amount)
                {
                    //call notification service when impl
                }
                savingsAccount!.TransferBalance(amount, false);
            }
        }
        else if (TryGetBankAccount(userID, accountNumber, out var bankAccount))
        {
            if (bankAccount!.GetThresholdNotification(amount, TransactionType.Withdrawal) <= amount)
            {
                //call notification service when impl
            }
            bankAccount.TransferBalance(amount, false);
        }
    }

    private bool TryGetBankAccount(int userID, int accountNumber, out BankAccount? bankAccount)
    {
        bankAccount = null;

        var bankAccounts = accountManager.BankAccountDictionary[userID];

        if (bankAccounts == null)
        {
            return false;
        }

        bankAccount = bankAccounts.Find(x => x.GetBankAccountNumber() == accountNumber);

        if (bankAccount == null)
        {
            return false;
        }

        return true;
    }

    private bool TryGetSavingsAccount(int userID, int accountNumber, out SavingsAccount? savingsAccount)
    {
        savingsAccount = null;

        var bankAccounts = accountManager.BankAccountDictionary[userID];

        if (bankAccounts == null)
        {
            return false;
        }

        var savingsAccounts = bankAccounts?.FirstOrDefault(x => x.GetUserId() == userID).SavingAccounts;

        if (savingsAccounts == null)
        {
            return false;
        }

        savingsAccount = savingsAccounts.FirstOrDefault(x => x.GetSavingsAccountNumber() == accountNumber);

        if (savingsAccount == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


}
