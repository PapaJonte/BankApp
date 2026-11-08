
using System.Xml.Linq;
using BankApp.GustafsClasses;
using BankApp.Jonathan;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transactions.Models;
using BankApp.Transactions.Transfers;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Crypto.Modes;

namespace BankApp.Transaction_Layer.Controller;

/// <summary>
/// Bridge class that interfaces with account manager and loan service from the transaction layer
/// </summary>
/// <param name=""></param>
/// <returns></returns>
internal class AccountBridge(AccountManager accountManager, LoanService loanService) : IAccountBridge
{

    /// <summary>
    /// Checks current balance on user account by account number and returns wether withdrawal is possible
    /// </summary>
    /// <param name="">
    /// int userID, id of user 
    /// int accountNumber BankAccount or SavingsAccount we want to check.
    /// decimal amount we want to withdraw
    /// </param>
    /// <returns></returns>
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

    /// <summary>
    /// Responsible for depositing given amount on either savings account or bank account.
    /// </summary>
    /// <param name="">
    /// int userID id of user connected to account.
    /// int accountNumber account number for savings- or checkings account.
    /// decimal amount amount of money to deposit.
    /// </param>
    /// <returns></returns>
    public void Deposit(int userID, int accountNumber, decimal amount)
    {
        if (TryGetSavingsAccount(userID, accountNumber, out var savingsAccount))
        {
            savingsAccount!.TransferBalance(amount, true);
        }
        else if (TryGetBankAccount(userID, accountNumber, out var bankAccount))
        {
            if (Math.Abs(bankAccount!.GetThresholdNotification(amount, TransactionType.Deposit)) <= amount)
            {
                //call notification service when impl
            }
            bankAccount.TransferBalance(amount, true);
        }
    }

    /// <summary>
    /// Withdraws given amount from savings- or bank account.
    /// </summary>
    /// <param name="">
    /// int userID id of user connected to account
    /// int accountNumber account number for savings- or bankaccount
    /// decimal amount total amount to withdraw
    /// </param>
    /// <returns></returns>
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

    /// <summary>
    /// Updates loanservice once a loan transaction is complete
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public void UpdateLoanService(TransactionDataLoanObject? loanObject)
    {
        if (loanObject == null) return;
        loanService.ConfirmLoanPayout(loanObject.LoanID, loanObject.TransactionDate);
    }

    /// <summary>
    /// Tries to get bank account based on account number and userID
    /// </summary>
    /// <param name="">
    /// int userID id of user connected to account.
    /// int accountNumber number for savings- or bank account
    /// out BankAccount bankAccount account found
    /// </param>
    /// <returns></returns>
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

    /// <summary>
    /// Try to get savings account based on userID and accountNumber, returns account as out param
    /// </summary>
    /// <param name="userID"> user id connected to account</param>
    /// <param name="accountNumber">accountnumber for savings- or bank acconut</param>
    /// <param name="savingsAccount">returned savings account if found</param>
    /// <returns></returns>
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
