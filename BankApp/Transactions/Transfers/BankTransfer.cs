using BankApp.Currencies;
using BankApp.Transaction_Layer.Controller;
using BankApp.Transactions.Models;

namespace BankApp.Transactions.Transfers;

public class BankTransfer(BankTransaction bankTransaction)
{

    /// <summary>
    /// Delegates transfer of money to BankTransfer and returns a status code.
    /// </summary>
    /// <param name="">
    /// AccountData originAccount holds relevant data for account where money is being withdrawed.
    /// AccountData targetAccount holds relevant data for account where money is being deposited.
    /// float transferAmount the amount of money requested to be transfered.
    /// Currency transferCurrency the currency that the money is requested to be transfered from.
    /// </param>
    /// <returns>
    /// StatusCode representing result of transfer
    /// </returns>
    public StatusCode TransferMoney(AccountData originAccount, AccountData targetAccount, decimal transferAmount, Currency transferCurrency)
    {
        StatusCode status = bankTransaction.TransferMoney(originAccount, targetAccount, transferAmount, transferCurrency);

        return status;
    }

    /// <summary>
    /// Handles loan transfer since it contains different structure for its data and the loanid its separated into its own function.
    /// </summary>
    /// <param name="">
    /// AccountData targetAccount the account we transfer loan into
    /// decimal loanAmount loan amount
    /// Currency loanCurrency currency we transfer loan to
    /// int loanID the id of the loan, used with callback to loanservice once transaction is complete.
    /// </param>
    /// <returns></returns>
    public StatusCode TransferLoan(AccountData targetAccount, decimal loanAmount, Currency loanCurrency, int loanID)
    {
        StatusCode status = bankTransaction.RequestLoanDeposit(targetAccount, loanAmount, loanCurrency, loanID);
        return status;
    }

    /// <summary>
    /// Request a deposition of money based on params, return status code
    /// </summary>
    /// <param name="">
    /// AccountData account aacount to deposit money to
    /// decimal depositAmount, value we add to bank account.
    /// </param>
    /// <returns>
    /// StatusCode represent status of deposit request.
    /// </returns>
    public StatusCode DepositMoney(AccountData account, decimal depositAmount)
    {
        return bankTransaction.RequestDeposit(account, depositAmount, account.Currency);
    }

    /// <summary>
    /// Request to withdraw money from account
    /// </summary>
    /// <param name="">
    /// AccountData account account to withdraw money from.
    /// decimal withdrawAmount amount to withdraw.
    /// </param>
    /// <returns>
    /// StatusCode represents status of withdrawal request.
    /// </returns>
    public StatusCode WithdrawMoney(AccountData account, decimal withdrawAmount)
    {
        StatusCode status = bankTransaction.WithdrawalStatus(account.UserID, account.AccountNumber, withdrawAmount);

        if (status == StatusCode.Successful)
        {
            status = bankTransaction.RequestWithdrawal(account.UserID, account.AccountNumber, withdrawAmount);
        }

        return status;
    }
}
