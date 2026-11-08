using BankApp.Transaction_Layer.DataModel;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transaction_Layer.Service;
using BankApp.Transactions.Models;
using BankApp.Transactions.Transfers;

namespace BankApp.Transactions.Processing;

public class StageTransaction
{
    private readonly TransactionScheduler transactionScheduler;
    private readonly TransactionRegistry transactionRegistry;
    private readonly IAccountBridge accountBridge;

    public StageTransaction(TransactionScheduler transactionScheduler, TransactionRegistry transactionRegistry, IAccountBridge accountBridge)
    {
        this.transactionScheduler = transactionScheduler;
        this.transactionRegistry = transactionRegistry;
        this.accountBridge = accountBridge;
        InitScheduler();
    }

    /// <summary>
    /// Returns all transactions in history for user
    /// </summary>
    /// <param name="userID"></param>
    /// <returns></returns>
    public List<TransactionDataObject> GetTransactionHistoryOnUserID(int userID)
    {
        return transactionRegistry.GetAllTransactionsOnUserID(userID);
    }
    //Returns all transactions in history for account.
    public List<TransactionDataObject> GetTransactionHistoryOnAccountID(int accountID)
    {
        return transactionRegistry.GetAllTransactionsOnAccountNumber(accountID);
    }

    /// <summary>
    /// return all completed transactions from registry
    /// </summary>
    /// <returns></returns>
    public List<TransactionDataObject> GetAllCompletedTransactions() => transactionRegistry.GetAllCompletedTransactions();

    /// <summary>
    /// Responsible for staging requested transaction of money. Only single transactions.
    /// Creates a transaction data object and registers it at the transaction registry.
    /// </summary>
    /// <param name="">
    /// string accountNumber, number of the account the transaction applies to.
    /// decimal amount, value the transaction requests to move.
    /// TransactionType transactionType, the type of transaction being staged (deposit or withdrawal).
    /// </param>
    public StatusCode Stage_SingleTransaction(TransactionDataObject transactionObject, bool stage)
    {
        StatusCode status = StatusCode.Successful;

        if (stage)
        {
            TransactionDataWrapper wrapper = transactionObject.TransactionType == TransactionType.Deposit ? TransactionDataWrapper.ForDeposit(transactionObject) : TransactionDataWrapper.ForWithdrawal(transactionObject);
            transactionRegistry.RegisterStagedTransaction(wrapper);
        }
        else
        {
            if (transactionObject.TransactionType == TransactionType.Deposit)
            {
                Deposit(transactionObject);
            }
            else if (transactionObject.TransactionType == TransactionType.Withdrawal)
            {
                if (accountBridge.CanWithdraw(transactionObject.userID, transactionObject.AccountNumber, transactionObject.Amount))
                {
                    WithDraw(transactionObject);
                }
                else
                {
                    status = StatusCode.Failed;
                }
            }
        }

        return status;
    }

    /// <summary>
    /// Responsible for staging transactiomn between two accounts
    /// </summary>
    /// <param name="">
    /// AccountData originAccount data object for withdrawal account
    /// AccountData targetAccount data object for deposit account
    /// decimal withdrawalAmount withdrawal amount
    /// decimal depositAmount deposit amount
    /// </param>
    /// <returns></returns>
    public void Stage_Transaction(AccountData originAccount, AccountData targetAccount, decimal withdrawalAmount, decimal depositAmount)
    {
        TransactionDataObject tdo_withdrawal = new(originAccount.UserID, originAccount.AccountNumber, withdrawalAmount, TransactionType.Withdrawal, DateTime.Now.Date);
        TransactionDataObject tdo_deposit = new(targetAccount.UserID, targetAccount.AccountNumber, depositAmount, TransactionType.Deposit, DateTime.Now.Date)
        {
            TransferSent = (originAccount.Currency, withdrawalAmount)
        };

        TransactionDataWrapper wrapper = TransactionDataWrapper.ForTransaction(tdo_withdrawal, tdo_deposit);

        transactionRegistry.RegisterStagedTransaction(wrapper);
    }

    #region Scheduler
    /// <summary>
    /// Responsible for starting the scheduler and hooking up the transaction processing callback.
    /// </summary>
    private void InitScheduler()
    {
        transactionScheduler.StartSchedule(OnSchedule);
    }

    /// <summary>
    /// Responsible for processing all staged transactions when the scheduler triggers.
    /// If transfer between accounts or withdrawal check current withdrawal status before actually moving money. If sinfle deposit just add the money directly.
    /// 
    /// </summary>
    private void OnSchedule()
    {
        var stagedTransactions = transactionRegistry.GetAllStagedTransactions(true);

        foreach (var s in stagedTransactions)
        {
            if (s.IsTransfer)
            {
                if (s.TryGetWithdrawalObject(out TransactionDataObject? withdrawalObj))
                {
                    if (withdrawalObj == null) continue;
                    if (accountBridge.CanWithdraw(withdrawalObj.userID, withdrawalObj.AccountNumber, withdrawalObj.Amount))
                    {
                        if (s.TryGetDepositObject(out TransactionDataObject? depositObject))
                        {
                            WithDraw(withdrawalObj);
                            Deposit(depositObject);
                        }
                    }
                }
            }
            else
            {
                if (s.TryGetDepositObject(out TransactionDataObject? depositObject))
                {
                    Deposit(depositObject);
                }
                else if (s.TryGetWithdrawalObject(out TransactionDataObject? withdrawalObject))
                {
                    if (withdrawalObject == null) continue;
                    if (accountBridge.CanWithdraw(withdrawalObject.userID, withdrawalObject.AccountNumber, withdrawalObject.Amount))
                    {
                        WithDraw(withdrawalObject);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Calls bridge to withdraw money from account and register transaction.
    /// </summary>
    /// <param name="withdrawalObject"></param>
    /// <exception cref="NullReferenceException"></exception>
    private void WithDraw(TransactionDataObject? withdrawalObject)
    {
        if (withdrawalObject == null) throw new NullReferenceException("Withdrawal object is null!");
        accountBridge.Withdraw(withdrawalObject.userID, withdrawalObject.AccountNumber, withdrawalObject.Amount);
        withdrawalObject = withdrawalObject with { TransactionDate = DateTime.UtcNow };
        transactionRegistry.RegisterSuccessfulTransaction(withdrawalObject);
    }
    
    /// <summary>
    /// Calls bridge to deposit money from account and register transaction. If deposition is loan
    /// call to update loan service.
    /// </summary>
    /// <param name="depositObject"></param>
    /// <exception cref="NullReferenceException"></exception>
    private void Deposit(TransactionDataObject? depositObject)
    {
        if (depositObject == null) throw new NullReferenceException("Deposit object is null!");
        accountBridge.Deposit(depositObject.userID, depositObject.AccountNumber, depositObject.Amount);
        depositObject = depositObject with { TransactionDate = DateTime.UtcNow };
        transactionRegistry.RegisterSuccessfulTransaction(depositObject);
        if(depositObject is TransactionDataLoanObject)
        {
            accountBridge.UpdateLoanService(depositObject as TransactionDataLoanObject);
        }
    }

    #endregion

}

