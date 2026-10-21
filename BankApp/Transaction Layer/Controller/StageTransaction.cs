using BankApp.Transaction_Layer.Data;
using BankApp.Transaction_Layer.DataModel;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transaction_Layer.Service;

namespace BankApp.Transaction_Layer.Controller;

public class StageTransaction
{
    private readonly TransactionScheduler transactionScheduler;
    private readonly TransactionRegistry transactionRegistry;
    private readonly Lock transactionLock = new();

    public StageTransaction(TransactionScheduler transactionScheduler, TransactionRegistry transactionRegistry)
    {
        this.transactionScheduler = transactionScheduler;
        this.transactionRegistry = transactionRegistry;
        InitScheduler();
    }

    public void Stage(string accountNumber, decimal amount, TransactionType transactionType)
    {
        TransactionDataObject tdo = new(accountNumber, amount, transactionType);
        transactionRegistry.RegisterStagedTransaction(tdo);
    }

    #region Scheduler
    private void InitScheduler()
    {
        transactionScheduler.StartSchedule(OnSchedule);
    }

    private void OnSchedule()
    {
        lock (transactionLock)
        {
            var stagedTransactions = transactionRegistry.GetAllStagedTransactions();
            foreach(var s in stagedTransactions)
            {
                //deposit and withdraw money
            }
        }
    }
    #endregion

}
