using System;
using BankApp.Transaction_Layer.DataModel;

namespace BankApp.Transaction_Layer.Interface;

public interface IAccountBridge
{
    public bool CanWithdraw(int userID, int accountNumber, decimal amount);
    public void Withdraw(int userID, int accountNumber, decimal amount);
    public void Deposit(int userID, int accountNumber, decimal amount);
    public void UpdateLoanService(TransactionDataLoanObject? loanObject);
}
