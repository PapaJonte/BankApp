using BankApp.Transaction_Layer.Controller;
using BankApp.Transaction_Layer.DataModel;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transaction_Layer.API;

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
        StatusCode transferCode = bankTransaction.TransferMoney(originAccount, targetAccount, transferAmount, transferCurrency);

        return transferCode;
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
        bankTransaction.RequestDeposit(account.UserID, account.AccountNumber, depositAmount);
        return StatusCode.Successful;
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
    public StatusCode WithdrawyMoney(AccountData account, decimal withdrawAmount)
    {
        StatusCode statusCode = bankTransaction.WithdrawalStatus(account.UserID, account.AccountNumber, withdrawAmount);

        if (statusCode == StatusCode.Successful)
        {
            bankTransaction.RequestWithdrawal(account.UserID, account.AccountNumber, withdrawAmount);
        }

        return statusCode;
    }
}
