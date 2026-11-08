using System;
using BankApp.Transactions.Models;

namespace BankApp.Transactions.Transfers;

public interface IAccountBridge
{
    public bool CanWithdraw(int userID, int accountNumber, decimal amount);
    public void Withdraw(int userID, int accountNumber, decimal amount);
    public void Deposit(int userID, int accountNumber, decimal amount);
    public void UpdateLoanService(TransactionDataLoanObject? loanObject);
}
