using System;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transactions.Models;

public record class TransactionDataObject(int userID, int AccountNumber, decimal Amount, TransactionType TransactionType, DateTime TransactionDate);