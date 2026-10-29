using System;
using BankApp.Transaction_Layer.API;
using BankApp.Transaction_Layer.DataModel;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transaction_Layer.Controller;

public class BankTransaction(StageTransaction stageTransaction, BankTransaction bankTransaction, ExchangeRate exchangeRateModule)
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

        /// <summary>
    /// Responsible for staging requested transfer of money. Handles and setup data so it is ready for transfer.
    /// Requests transaction at transaction module, depending on answer returns a status code.
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
        decimal withdrawalAmount = 0m, depositAmount = 0m;
        StatusCode statusCode;

        if (originAccount.Currency == transferCurrency) //calculate how much we will deposit
        {
            statusCode = exchangeRateModule.TryCalculateExchangeValue(transferCurrency, targetAccount.Currency, transferAmount, out depositAmount);
            withdrawalAmount = transferAmount;
        }
        else if (targetAccount.Currency == transferCurrency) //calculate how much we will withdraw
        {
            statusCode = exchangeRateModule.TryCalculateExchangeValue(targetAccount.Currency, originAccount.Currency, transferAmount, out withdrawalAmount);
            depositAmount = transferAmount;
        }
        else//if transfer currency is outside both of accounts it is invalid.
        {
            statusCode = StatusCode.Invalid;
        }

        if (statusCode == StatusCode.Successful)
        {
            //verify account balance
            bankTransaction.RequestDeposit(targetAccount.AccountNumber, depositAmount);
            bankTransaction.RequestWithdrawal(originAccount.AccountNumber, withdrawalAmount);
        }

        return statusCode;
    }
}

