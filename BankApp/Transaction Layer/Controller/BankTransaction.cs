using System;
using BankApp.Pull_The_Sack.API;
using BankApp.Transaction_Layer.API;
using BankApp.Transaction_Layer.DataModel;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transaction_Layer.Interface;
using BankApp.Transaction_Layer.Service;

namespace BankApp.Transaction_Layer.Controller;

public class BankTransaction(StageTransaction stageTransaction, ExchangeRate exchangeRateModule, IAccountBridge accountBridge)
{

    /// <summary>
    /// Responsible for requesting a deposit of money into an account.
    /// Stages a deposit transaction at the transaction module.
    /// </summary>
    /// <param name="">
    /// string accountNumber, number of the account we deposit money to.
    /// decimal amount, value we add to the bank account.
    /// </param>
    public void RequestDeposit(int userID, int accountNumber, decimal amount)
    {
        stageTransaction.Stage_Single(userID, accountNumber, amount, TransactionType.Deposit);
    }

    /// <summary>
    /// Responsible for requesting a withdrawal of money from an account.
    /// Stages a withdrawal transaction at the transaction module.
    /// </summary>
    /// <param name="">
    /// string accountNumber, number of the account we withdraw money from.
    /// decimal amount, value we remove from the bank account.
    /// </param>
    public void RequestWithdrawal(int userID, int accountNumber, decimal amount)
    {
        stageTransaction.Stage_Single(userID, accountNumber, amount, TransactionType.Withdrawal);
    }

    private void RequestTransfer(AccountData origin, AccountData target, decimal withdrawalAmount, decimal depositAmount)
    {
        stageTransaction.Stage_Transaction(origin, target, withdrawalAmount, depositAmount);
    }

    public StatusCode WithdrawalStatus(int userID, int accountNumber, decimal amount)
    {
        StatusCode statusCode = accountBridge.CanWithdraw(userID, accountNumber, amount) ? StatusCode.Successful : StatusCode.Failed;
        return statusCode;
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

        //Check if withdraw possible
        if (statusCode == StatusCode.Successful)
        {
            statusCode = WithdrawalStatus(originAccount.UserID, originAccount.AccountNumber, withdrawalAmount);
        }

        if (statusCode == StatusCode.Successful)
        {
            RequestTransfer(originAccount, targetAccount, withdrawalAmount, depositAmount);
        }

        return statusCode;
    }
}

