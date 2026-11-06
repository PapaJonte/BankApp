
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
        if(TryGetBankAccounts(userID, accountNumber, out (BankAccount, SavingsAccount)? accs))
        {
            if (accs?.Item1.GetThresholdNotification(amount, TransactionType.Deposit) >= amount)
            {
                //call notification service when impl
            }
            accs?.Item2.TransferBalance(amount, true);
        }
    }

    public void Withdraw(int userID, int accountNumber, decimal amount)
    {
        if (TryGetBankAccounts(userID, accountNumber, out (BankAccount, SavingsAccount)? accs))
        {
            if (accs?.Item1.GetThresholdNotification(amount, TransactionType.Withdrawal) >= amount)
            {
                //call notification service when impl
            }
            accs?.Item2.TransferBalance(amount, false);
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

    private bool TryGetBankAccounts(int userID, int accountNumber, out (BankAccount, SavingsAccount)? bankTouple)
    {
        bankTouple = null;
        BankAccount? bankAccount = null;
        SavingsAccount? savingsAccount;
        var bankAccounts = accountManager.BankAccountDictionary[userID];

        if (bankAccounts == null)
        {
            return false;
        }

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

        if (bankAccount == null)
        {
            return false;
        }

        savingsAccount = bankAccount.SavingAccounts.FirstOrDefault(x => x.GetSavingsAccountNumber() == accountNumber);

        if (savingsAccount == null)
        {
            return false;
        }

        bankTouple = new(bankAccount, savingsAccount);

        return true;
    }

}
