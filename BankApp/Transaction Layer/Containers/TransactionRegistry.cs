using System;
using BankApp.Transaction_Layer.DataModel;

namespace BankApp.Transaction_Layer.Data;

public class TransactionRegistry
{
    private readonly Queue<TransactionDataObject> stagedTransactions = new(); //we'll use queue in case maybe we'd like to limit amount of transactions at a later stage.

    /// <summary>
    /// Responsible for staging a requested transaction so it is ready to be processed later.
    /// Adds the given transaction to the back of the staged transaction queue.
    /// </summary>
    /// <param name="">
    /// TransactionDataObject transactionData holds the relevant data for the transaction being staged.
    /// </param>
    public void RegisterStagedTransaction(TransactionDataObject transactionData)
    {
        stagedTransactions.Enqueue(transactionData);
    }

    /// <summary>
    /// Responsible for retrieving and clearing all staged transactions at once.
    /// Returns a copy of the queue contents and empties the staged transaction queue.
    /// </summary>
    /// <returns>
    /// List of TransactionDataObject representing all currently staged transactions
    /// </returns>
    public List<TransactionDataObject> GetAllStagedTransactions()
    {
        List<TransactionDataObject> transactions = [.. stagedTransactions];
        return transactions;
    }

    /// <summary>
    /// Clears current staged transaction queue
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public void ClearStagedTransactions() => stagedTransactions.Clear();

    /// <summary>
    /// Responsible for retrieving the next staged transaction in order.
    /// Removes the transaction from the front of the staged transaction queue.
    /// </summary>
    /// <returns>
    /// TransactionDataObject representing the next staged transaction
    /// </returns>
    public TransactionDataObject GetNextStagedTransaction()
    {
        var stagedTransaction = stagedTransactions.Dequeue();
        return stagedTransaction;
    }

}

