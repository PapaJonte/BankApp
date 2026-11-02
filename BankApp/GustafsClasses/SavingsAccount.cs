using BankApp.Transaction_Layer.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    internal class SavingsAccount
    {
        private decimal Balance; //Ammount in the account.
        private Currency Currency; //Currency of Account.

        private decimal Interest;
        private int SavingsAccountNumber; //For access to specific savings accounts.

        public SavingsAccount (decimal Balance, Currency Currency, decimal Interest, int SavingsAccountNumber) : base ( )
        {
            this.Balance = Balance;
            this.Currency = Currency;
            this.Interest = Interest;
            this.SavingsAccountNumber = SavingsAccountNumber;
        }
        //For transfer of balance. Bool determines if you add or remove balance.
        public void TransferBalance(int transferSum, bool addMore)
        {
            if (addMore == true)
            {
                Balance = Balance + transferSum;
            }
            else
            {
                Balance = Balance - transferSum;
            }
        }

        //Method that returns account balance.
        public decimal GetBalance()
        {
            return Balance;
        }
        public Currency GetCurrency()
        {
            return Currency;
        }
        //Method that returns interest.
        public decimal GetInterest()
        {
            return Interest;
        }
    }
}
