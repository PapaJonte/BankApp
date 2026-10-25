using BankApp.Transaction_Layer.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    internal class SavingsAccount : BankAccount
    {
        private decimal Balance; //Ammount in the account.
        private Currency Currency; //Currency of Account.
        private decimal Interest;

        public SavingsAccount (decimal balance, Currency currency, decimal interest) : base ( )
        {
            balance = Balance;
            currency = Currency;
            interest = Interest;
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
