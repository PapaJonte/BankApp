using BankApp.Transaction_Layer.Controller;
using BankApp.Transaction_Layer.DataModel;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transaction_Layer.API;

public class BankTransfer(BankTransaction bankTransaction, ExchangeRate exchangeRateModule)
{

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
        decimal withdrawalAmount, depositAmount;
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
            //request transaction
        }

        return statusCode;
    }
    
    /// <summary>
    /// Request a deposition of money based on params, return status code
    /// </summary>
    /// <param name="">
    /// string accountNumber, number of account we deposit money to
    /// decimal depositAmount, value we add to bank account.
    /// </param>
    /// <returns></returns>
    public StatusCode DepositMoney(string accountNumber, decimal depositAmount)
    {
        bankTransaction.RequestDeposit(accountNumber, depositAmount);
        return StatusCode.Successful;
    }
}
