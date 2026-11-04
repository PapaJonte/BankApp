using System;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transaction_Layer.DataModel;

public readonly record struct TransactionDataObject(int userID, int AccountNumber, decimal Amount, TransactionType TransactionType, DateTime TransactionDate);