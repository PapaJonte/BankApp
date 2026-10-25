using BankApp.Transaction_Layer.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    internal class SavingsAccount : BankAccount
    {
        private decimal balance; //Ammount in the account.
        private Currency currency; //Currency of Account.
        private decimal interest;

        public SavingsAccount (decimal Balance, Currency Currency, decimal Interest) : base ( )
        {
            Balance = balance;
            Currency = currency;
            Interest = interest;
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
    }
}
