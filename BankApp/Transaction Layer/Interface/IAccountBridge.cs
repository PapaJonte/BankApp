using System;

namespace BankApp.Transaction_Layer.Interface;

public interface IAccountBridge
{
    public bool CanWithdraw(int userID, int accountNumber, decimal amount);
    public void Withdraw(int userID, int accountNumber, decimal amount);
    public void Deposit(int userID, int accountNumber, decimal amount);
}
