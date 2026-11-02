using System;
using System.Transactions;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transaction_Layer.DataModel;


/// <summary>
/// Wrapper round TransactionDataObject for storage inside transaction registry. Keep the data for single withdrawal/deposit or for the transaction between accounts. 
/// </summary>
/// <param name=""></param>
/// <returns></returns>

public readonly struct TransactionDataWrapper(TransactionDataObject[] dataObjects)
{

    public readonly bool IsTransfer => _dataObjects.Length == 2;
    private readonly TransactionDataObject[] _dataObjects = dataObjects;


    public readonly TransactionDataObject GetDataObject() => _dataObjects[0];

    public readonly TransactionDataObject[] GetAllTransactionObjects() => _dataObjects;

    /// <summary>
    /// Try get data object with transaction type set to withdrawal
    /// </summary>
    /// <param name="">
    /// out TransactionDataObject dataObject object found for withdrawal
    /// </param>
    /// <returns>
    /// bool flag if object is found.
    /// </returns>
    public readonly bool TryGetWithdrawalObject(out TransactionDataObject dataObject)
    {
        bool hasWithdrawal = false;
        dataObject = default;

        foreach (var d in _dataObjects)
        {
            if (d.TransactionType == TransactionType.Withdrawal)
            {
                hasWithdrawal = true;
                dataObject = d;
            }
        }

        return hasWithdrawal;
    }

    /// <summary>
    /// Try get data object with transaction type set to deposit
    /// </summary>
    /// <param name="">
    /// out TransactionDataObject dataObject deposit object found
    /// </param>
    /// <returns>
    /// bool flag for finding object or not.
    /// </returns>
    public readonly bool TryGetDepositObject(out TransactionDataObject dataObject)
    {
        bool hasDeposit = false;
        dataObject = default;

        foreach (var d in _dataObjects)
        {
            if (d.TransactionType == TransactionType.Deposit)
            {
                hasDeposit = true;
                dataObject = d;
            }
        }

        return hasDeposit;
    }

    /// <summary>
    /// Factory for single data object, with deposit transaction type.
    /// </summary>
    /// <param name="">
    /// TransactionDataObject transactionObject object holding data for single deposit.
    /// </param>
    /// <returns></returns>
    public static TransactionDataWrapper ForDeposit(TransactionDataObject transactionObject)
    {
        if (transactionObject.TransactionType != TransactionType.Deposit)
        {
            throw new FormatException("transaction object passed is not a deposit object");
        }

        TransactionDataWrapper tdw = new([transactionObject]);

        return tdw;
    }

    /// <summary>
    /// Factory for single data object, with withdrawal transaction type.
    /// </summary>
    /// <param name="">
    /// TransactionDataObject transactionObject object holding data for single withdrawal
    /// </param>
    /// <returns></returns>
    public static TransactionDataWrapper ForWithdrawal(TransactionDataObject transactionObject)
    {
        if (transactionObject.TransactionType != TransactionType.Withdrawal)
        {
            throw new FormatException("transaction object passed is not a withdrawal object");
        }

        TransactionDataWrapper tdw = new([transactionObject]);

        return tdw;
    }

    /// <summary>
    /// Factory for transaction data between two transaction data objects.
    /// </summary>
    /// <param name="">
    /// TransactionDataObject withdrawalObject object holding data for withdrawal
    /// TransactionDataObject depositObject object holding data for deposit
    /// </param>
    /// <returns></returns>
    public static TransactionDataWrapper ForTransaction(TransactionDataObject withdrawalObject, TransactionDataObject depositObject)
    {
        if (withdrawalObject.TransactionType != TransactionType.Withdrawal)
        {
            throw new FormatException("transaction object passed is not a withdrawal object");
        }
        if (depositObject.TransactionType != TransactionType.Deposit)
        {
            throw new FormatException("transaction object passed is not a deposit object");
        }

        TransactionDataWrapper tdw = new([withdrawalObject, depositObject]);

        return tdw;
    }

}
