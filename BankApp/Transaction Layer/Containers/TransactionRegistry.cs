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
    /// Responsible for retrieving all staged transactions at once.
    /// Returns a copy of the queue contents, leaving the queue unchanged.
    /// Thread-safe: acquires the registry lock while copying the queue.
    /// </summary>
    /// <returns>
    /// List of TransactionDataObject representing all currently staged transactions
    /// </returns>
    public List<TransactionDataObject> GetAllStagedTransactions()
    {
        lock (registryLock)
        {
            List<TransactionDataObject> transactions = [.. stagedTransactions];
            return transactions;
        }
    }

    /// <summary>
    /// Responsible for clearing the current staged transaction queue.
    /// Removes all staged transactions from the queue.
    /// Thread-safe: acquires the registry lock while clearing the queue.
    /// </summary>
    public void ClearStagedTransactions()
    {
        lock (registryLock)
        {
            stagedTransactions.Clear();
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

