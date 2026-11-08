using System;
using BankApp.Currencies;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transactions.Models;

public record class TransactionDataObject(int userID, int AccountNumber, decimal Amount, TransactionType TransactionType, DateTime TransactionDate)
{
    // Only a transfer's credit entry carries the original amount sent
    public (Currency currency, decimal Amount)? TransferSent { get; init; }
}