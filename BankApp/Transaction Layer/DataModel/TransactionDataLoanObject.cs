using System;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transaction_Layer.DataModel;

public record class TransactionDataLoanObject(int UserID, int AccountNumber, decimal Amount, TransactionType TransactionType, DateTime TransactionDate, int LoanID) : TransactionDataObject(UserID, AccountNumber, Amount, TransactionType, TransactionDate);
