using System;
using BankApp.Transaction_Layer.DataModel;

namespace BankApp.Transaction_Layer.Data;

public class TransactionRegistry
{
    private readonly Queue<TransactionDataObject> stagedTransactions = new(); //we'll use queue in case maybe we'd like to limit amount of transactions at a later stage.

    public void RegisterStagedTransaction(TransactionDataObject transactionData)
    {
        stagedTransactions.Enqueue(transactionData);
    }

    public List<TransactionDataObject> GetAllStagedTransactions()
    {
        List<TransactionDataObject> transactions = [.. stagedTransactions];
        stagedTransactions.Clear();
        return transactions;
    }

    public TransactionDataObject GetNextStagedTransaction()
    {
        var stagedTransaction = stagedTransactions.Dequeue();
        return stagedTransaction;
    }

}
