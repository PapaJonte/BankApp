using System;
using System.Transactions;
using BankApp.Transaction_Layer.DataModel;

namespace BankApp.Transaction_Layer.Data;

public class TransactionRegistry
{

    private readonly Queue<TransactionDataWrapper> stagedTransactions = new(); //we'll use queue in case maybe we'd like to limit amount of transactions at a later stage.
    private readonly List<TransactionDataObject> transactionHistory = [];
    private readonly Lock registryLock = new();
    /// <summary>
    /// Responsible for staging a requested transaction so it is ready to be processed later.
    /// Adds the given transaction to the back of the staged transaction queue.
    /// Thread-safe: acquires the registry lock before modifying the queue.
    /// </summary>
    /// <param name="">
    /// TransactionDataObject transactionData holds the relevant data for the transaction being staged.
    /// </param>
    public void RegisterStagedTransaction(TransactionDataWrapper transactionDataWrapper)
    {
        lock (registryLock)
        {
            stagedTransactions.Enqueue(transactionDataWrapper);
        }
    }

    /// <summary>
    /// Adds a transaction object from a successful transaction into the history list
    /// </summary>
    /// <param name="">
    /// TransactionDataObject transactionObject object with stored successful transaction data
    /// </param>
    /// <returns></returns>
    public void RegisterSuccessfulTransaction(TransactionDataObject transactionObject)
    {
        transactionHistory.Add(transactionObject);
    }
    /// <summary>
    /// return all completed transactions as new list.
    /// </summary>
    /// <returns></returns>
    public List<TransactionDataObject> GetAllCompletedTransactions() => [.. transactionHistory];
    public List<TransactionDataObject> GetAllTransactionsOnUserID(int userID)
    {
        return [.. transactionHistory.Where(x => x.userID == userID)];
    }

    public List<TransactionDataObject> GetAllTransactionsOnAccountNumber(int accountNumber)
    {
        return [.. transactionHistory.Where(x => x.AccountNumber == accountNumber)];
    }

    /// <summary>
    /// Responsible for retrieving all staged transactions at once.
    /// Returns a copy of the queue contents, leaving the queue unchanged.
    /// Thread-safe: acquires the registry lock while copying the queue.
    /// </summary>
    /// <returns>
    /// List of TransactionDataObject representing all currently staged transactions
    /// </returns>
    public List<TransactionDataWrapper> GetAllStagedTransactions(bool clearOnReturn)
    {
        lock (registryLock)
        {
            List<TransactionDataWrapper> transactions = [.. stagedTransactions];
            if(clearOnReturn)
            {
                stagedTransactions.Clear();
            }
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
    public TransactionDataWrapper GetNextStagedTransaction()
    {
        lock (registryLock)
        {
            var stagedTransaction = stagedTransactions.Dequeue();
            return stagedTransaction;
        }
    }

}

