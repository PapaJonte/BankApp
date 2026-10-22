using System;
using BankApp.Transaction_Layer.DataModel;

namespace BankApp.Transaction_Layer.Data;

public class TransactionRegistry
{

    private readonly Queue<TransactionDataObject> stagedTransactions = new(); //we'll use queue in case maybe we'd like to limit amount of transactions at a later stage.
    private readonly Lock registryLock = new();
    /// <summary>
    /// Responsible for staging a requested transaction so it is ready to be processed later.
    /// Adds the given transaction to the back of the staged transaction queue.
    /// Thread-safe: acquires the registry lock before modifying the queue.
    /// </summary>
    /// <param name="">
    /// TransactionDataObject transactionData holds the relevant data for the transaction being staged.
    /// </param>
    public void RegisterStagedTransaction(TransactionDataObject transactionData)
    {
        lock (registryLock)
        {
            stagedTransactions.Enqueue(transactionData);
        }
    }

    /// <summary>
    /// Responsible for atomically retrieving and clearing all staged transactions at once.
    /// Returns a copy of the queue contents and empties the queue in a single locked operation,
    /// so no transaction staged during processing can be lost.
    /// Thread-safe: acquires the registry lock for the whole get-and-clear operation.
    /// </summary>
    /// <returns>
    /// List of TransactionDataObject representing all currently staged transactions
    /// </returns>
    public List<TransactionDataObject> DrainStagedTransactions()
    {
        lock (registryLock)
        {
            List<TransactionDataObject> transactions = [.. stagedTransactions];
            stagedTransactions.Clear();
            return transactions;
        }
    }

    /// <summary>
    /// Responsible for retrieving the next staged transaction in order.
    /// Removes the transaction from the front of the staged transaction queue.
    /// Thread-safe: acquires the registry lock while dequeuing.
    /// </summary>
    /// <returns>
    /// TransactionDataObject representing the next staged transaction
    /// </returns>
    public TransactionDataObject GetNextStagedTransaction()
    {
        lock (registryLock)
        {
            var stagedTransaction = stagedTransactions.Dequeue();
            return stagedTransaction;
        }
    }

}



