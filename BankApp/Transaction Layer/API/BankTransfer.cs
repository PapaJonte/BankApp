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
