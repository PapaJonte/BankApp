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
        private int SavingsAccountNumber;

        public SavingsAccount (decimal Balance, Currency Currency, decimal Interest, int SavingsAccountNumber) : base ( )
        {
            this.Balance = Balance;
            this.Currency = Currency;
            this.Interest = Interest;
            this.SavingsAccountNumber = SavingsAccountNumber;
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
