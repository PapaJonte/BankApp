using System;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transaction_Layer.DataModel;

public record class TransactionDataObject(int userID, int AccountNumber, decimal Amount, TransactionType TransactionType, DateTime TransactionDate);