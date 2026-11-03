using BankApp.Transaction_Layer.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.GustafsClasses
{
    internal class SavingsAccount
    {
        private decimal balance; //Ammount in the account.
        private Currency currency; //Currency of Account.

        private decimal interest;
        private int savingsAccountNumber; //For access to specific savings accounts.

        public SavingsAccount (decimal Balance, Currency Currency, decimal Interest, int SavingsAccountNumber)
        {
            Balance = balance;
            Currency = currency;
            Interest = interest;
            SavingsAccountNumber = savingsAccountNumber;
        }
        //For transfer of balance. Bool determines if you add or remove balance.
        public void TransferBalance(decimal transferSum, bool addMore)
        {
            if (addMore == true)
            {
                balance = balance + transferSum;
            }
            else
            {
                balance = balance - transferSum;
            }
        }

        //Method that returns account balance.
        public decimal GetBalance()
        {
            return balance;
        }
        public Currency GetCurrency()
        {
            return currency;
        }
        //Method that returns interest.
        public decimal GetInterest()
        {
            return interest;
        }

        public int GetSavingsAccountNumber()
        {
            return savingsAccountNumber;
        }
    }
}
