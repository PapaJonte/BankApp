using BankApp.Transaction_Layer.Controller;
using BankApp.Transaction_Layer.DataModel;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transaction_Layer.API;

public class BankTransfer(BankTransaction bankTransaction, ExchangeRate exchangeRateModule)
{
    private readonly BankTransaction _bankTransactionModule = bankTransaction;
    private readonly ExchangeRate _exchangeRateModule = exchangeRateModule;

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
    public StatusCode TransferMoney(AccountData originAccount, AccountData targetAccount, float transferAmount, Currency transferCurrency)
    {
        //fetch exchange rate value from exhangerate module
        //Verify account balance
        //Request transaction
        //return StatusCode
        return StatusCode.Successful;
    }
    
    public StatusCode DepositMoney(string accountNumber, float depositAmount)
    {
        //request deposition
        //return StatusCode

        return StatusCode.Successful;
    }
}
