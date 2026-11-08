using BankApp.Currencies;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transactions.Models;
using BankApp.Transactions.Processing;
using BankApp.Transactions.Transfers;

namespace BankApp.Transaction_Layer.Controller;

public class BankTransaction(StageTransaction stageTransaction, ExchangeRate exchangeRateModule, IAccountBridge accountBridge)
{

    /// <summary>
    /// Responsible for requesting a deposit of money into an account.
    /// Stages a deposit transaction at the transaction module.
    /// </summary>
    /// <param name="">
    /// AccountData account data for target account
    /// decimal amount, value we add to the bank account.
    /// Currency currency represents the currency amount is in, will be converted to target account currency
    /// </param>
    public StatusCode RequestDeposit(AccountData targetAccount, decimal amount, Currency currency)
    {
        StatusCode status = StatusCode.Successful;
        if (targetAccount.Currency != currency)
        {
            status = exchangeRateModule.TryCalculateExchangeValue(currency, targetAccount.Currency, amount, out decimal convertedAmount);
            if (status == StatusCode.Successful)
            {
                amount = convertedAmount;
            }
        }

        if (status == StatusCode.Successful)
        {
            TransactionDataObject transactionObject = new(targetAccount.UserID, targetAccount.AccountNumber, amount, TransactionType.Deposit, DateTime.UtcNow);
            status = stageTransaction.Stage_SingleTransaction(transactionObject, true);
        }

        return status;
    }

    /// <summary>
    /// Request deposit loan to target account
    /// </summary>
    /// <param name="targetAccount">Account to receive loan</param>
    /// <param name="amount">Amount to deposit</param>
    /// <param name="currency">Currency loan is deposited in</param>
    /// <param name="loanID">id of loan</param>
    /// <returns></returns>
    public StatusCode RequestLoanDeposit(AccountData targetAccount, decimal amount, Currency currency, int loanID)
    {
        StatusCode status = StatusCode.Successful;
        if (targetAccount.Currency != currency)
        {
            status = exchangeRateModule.TryCalculateExchangeValue(currency, targetAccount.Currency, amount, out decimal convertedAmount);
            if (status == StatusCode.Successful)
            {
                amount = convertedAmount;
            }
        }
        if (status == StatusCode.Successful)
        {
            TransactionDataLoanObject transactionObject = new(targetAccount.UserID, targetAccount.AccountNumber, amount, TransactionType.Deposit, DateTime.UtcNow, loanID);
            status = stageTransaction.Stage_SingleTransaction(transactionObject, true);
        }

        return status;
    }

    /// <summary>
    /// Responsible for requesting a withdrawal of money from an account.
    /// Stages a withdrawal transaction at the transaction module.
    /// </summary>
    /// <param name="">
    /// string accountNumber, number of the account we withdraw money from.
    /// decimal amount, value we remove from the bank account.
    /// </param>
    public StatusCode RequestWithdrawal(int userID, int accountNumber, decimal amount)
    {
        TransactionDataObject withdrawObject = new(userID, accountNumber, amount, TransactionType.Withdrawal, DateTime.UtcNow);
        StatusCode status = stageTransaction.Stage_SingleTransaction(withdrawObject, false);
        return status;
    }

    /// <summary>
    /// Transfer money between two accounts
    /// </summary>
    /// <param name="origin">account money withdraw from</param>
    /// <param name="target">account money deposit in</param>
    /// <param name="withdrawalAmount">amount to withdraw</param>
    /// <param name="depositAmount">amount to deposit</param>
    private void RequestTransfer(AccountData origin, AccountData target, decimal withdrawalAmount, decimal depositAmount)
    {
        stageTransaction.Stage_Transaction(origin, target, withdrawalAmount, depositAmount);
    }

    /// <summary>
    /// Checks if account has sufficent funding for withdrawal and return status code
    /// </summary>
    /// <param name="userID">id of user connected to account</param>
    /// <param name="accountNumber">account numbver, either savings- or bank account</param>
    /// <param name="amount">amount to check withdraw</param>
    /// <returns></returns>
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

        if (originAccount.Currency == transferCurrency && targetAccount.Currency == transferCurrency)
        {
            // Both accounts use the same currency, so the amounts are equal
            withdrawalAmount = transferAmount;
            depositAmount = transferAmount;
            statusCode = StatusCode.Successful;
        }
        else if (originAccount.Currency == transferCurrency)
        {
            // The customer entered the amount to send. Calculate what the recipient gets
            withdrawalAmount = transferAmount;
            statusCode = exchangeRateModule.TryCalculateExchangeValue(originAccount.Currency, targetAccount.Currency, transferAmount, out depositAmount);
        }
        else if (targetAccount.Currency == transferCurrency)
        {
            // The customer entered the amount the recipient should get. Calculate what to debit
            depositAmount = transferAmount;
            statusCode = exchangeRateModule.TryCalculateExchangeValue(targetAccount.Currency, originAccount.Currency, transferAmount, out withdrawalAmount);
        }
        else
        {
            // The entered currency must match one of the accounts
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

