
using System.Xml.Linq;
using BankApp.GustafsClasses;
using BankApp.Jonathan;
using BankApp.Transaction_Layer.DataModel;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transaction_Layer.Interface;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Crypto.Modes;

namespace BankApp.Transaction_Layer.Controller;

internal class AccountBridge(AccountManager accountManager, LoanService loanService) : IAccountBridge
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
            savingsAccount!.TransferBalance(amount, true);
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
            savingsAccount!.TransferBalance(amount, false);
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

    public void UpdateLoanService(TransactionDataLoanObject? loanObject)
    {
        if (loanObject == null) return;
        loanService.ConfirmLoanPayout(loanObject.LoanID, loanObject.TransactionDate);
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

        return bankAccount != null;
    }

    private bool TryGetSavingsAccount(int userID, int accountNumber, out SavingsAccount? savingsAccount)
    {
        savingsAccount = null;

        if (!accountManager.BankAccountDictionary.TryGetValue(userID, out var bankAccounts))
        {
            return false;
        }

        foreach (BankAccount bankAccount in bankAccounts)
        {
            savingsAccount = bankAccount.SavingAccounts.Find(account => account.GetSavingsAccountNumber() == accountNumber);

            if (savingsAccount != null)
            {
                return true;
            }
        }

        return false;
    }


}
