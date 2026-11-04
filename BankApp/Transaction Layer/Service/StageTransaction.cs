using BankApp.Transaction_Layer.Data;
using BankApp.Transaction_Layer.DataModel;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transaction_Layer.Interface;

namespace BankApp.Transaction_Layer.Service;

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


    public List<TransactionDataObject> GetTransactionHistoryOnUserID(int userID)
    {
        return transactionRegistry.GetAllTransactionsOnUserID(userID);
    }

    public List<TransactionDataObject> GetTransactionHistoryOnAccountID(int accountID)
    {
        return transactionRegistry.GetAllTransactionsOnAccountNumber(accountID);
    }

    /// <summary>
    /// Responsible for staging requested transaction of money. Only single transactions.
    /// Creates a transaction data object and registers it at the transaction registry.
    /// </summary>
    /// <param name="">
    /// string accountNumber, number of the account the transaction applies to.
    /// decimal amount, value the transaction requests to move.
    /// TransactionType transactionType, the type of transaction being staged (deposit or withdrawal).
    /// </param>
    public void Stage_Single(int userID, int accountNumber, decimal amount, TransactionType transactionType)
    {
        TransactionDataObject tdo = new(userID, accountNumber, amount, transactionType, DateTime.Now.Date);
        TransactionDataWrapper wrapper = transactionType == TransactionType.Deposit ? TransactionDataWrapper.ForDeposit(tdo) : TransactionDataWrapper.ForWithdrawal(tdo);
        transactionRegistry.RegisterStagedTransaction(wrapper);
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
        TransactionDataObject tdo_deposit = new(targetAccount.UserID, targetAccount.AccountNumber, depositAmount, TransactionType.Deposit, DateTime.Now.Date);

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
        var stagedTransactions = transactionRegistry.GetAllStagedTransactions();

        foreach (var s in stagedTransactions)
        {
            if (s.IsTransfer)
            {
                if (s.TryGetWithdrawalObject(out TransactionDataObject withdrawalObj))
                {
                    if (accountBridge.CanWithdraw(withdrawalObj.userID, withdrawalObj.AccountNumber, withdrawalObj.Amount))
                    {
                        if (s.TryGetDepositObject(out TransactionDataObject depositObject))
                        {
                            accountBridge.Withdraw(withdrawalObj.userID, withdrawalObj.AccountNumber, withdrawalObj.Amount);
                            accountBridge.Deposit(depositObject.userID, depositObject.AccountNumber, depositObject.Amount);

                            transactionRegistry.RegisterSuccessfulTransaction(withdrawalObj);
                            transactionRegistry.RegisterSuccessfulTransaction(depositObject);
                        }
                    }
                }
            }
            else
            {
                if (s.TryGetDepositObject(out TransactionDataObject depositObject))
                {
                    accountBridge.Deposit(depositObject.userID, depositObject.AccountNumber, depositObject.Amount);
                    transactionRegistry.RegisterSuccessfulTransaction(depositObject);
                }
                else if (s.TryGetWithdrawalObject(out TransactionDataObject withdrawalObject))
                {
                    if (accountBridge.CanWithdraw(withdrawalObject.userID, withdrawalObject.AccountNumber, withdrawalObject.Amount))
                    {
                        accountBridge.Withdraw(withdrawalObject.userID, withdrawalObject.AccountNumber, withdrawalObject.Amount);
                        transactionRegistry.RegisterSuccessfulTransaction(withdrawalObject);
                    }
                }
            }
        }

        transactionRegistry.ClearStagedTransactions();
    }

    #endregion

}

