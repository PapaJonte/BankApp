using BankApp.Transaction_Layer.Data;
using BankApp.Transaction_Layer.DataModel;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transaction_Layer.Service;

namespace BankApp.Transaction_Layer.Controller;

public class StageTransaction
{
    private readonly TransactionScheduler transactionScheduler;
    private readonly TransactionRegistry transactionRegistry;

    public StageTransaction(TransactionScheduler transactionScheduler, TransactionRegistry transactionRegistry)
    {
        this.transactionScheduler = transactionScheduler;
        this.transactionRegistry = transactionRegistry;
        InitScheduler();
    }

    /// <summary>
    /// Responsible for staging requested transaction of money. Handles and setup data so it is ready for processing.
    /// Creates a transaction data object and registers it at the transaction registry.
    /// </summary>
    /// <param name="">
    /// string accountNumber, number of the account the transaction applies to.
    /// decimal amount, value the transaction requests to move.
    /// TransactionType transactionType, the type of transaction being staged (deposit or withdrawal).
    /// </param>
    public void Stage(string accountNumber, decimal amount, TransactionType transactionType)
    {
        TransactionDataObject tdo = new(accountNumber, amount, transactionType);
        transactionRegistry.RegisterStagedTransaction(tdo);
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
    /// Locks processing to keep it thread-safe, then drains and iterates the staged transactions.
    /// </summary>
    private void OnSchedule()
    {
        var stagedTransactions = transactionRegistry.GetAllStagedTransactions();
        foreach (var s in stagedTransactions)
        {
            //deposit and withdraw money
        }
        transactionRegistry.ClearStagedTransactions();
    }
    #endregion

}

