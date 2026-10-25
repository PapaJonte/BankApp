using BankApp.Transaction_Layer.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{

    /*The place in the class hierarchy has been changed so that SavingsAccount is now a lower class of BankAccount.
     * Must be adapted to fit with the code of the API-folder.*/
    internal class SavingsAccount : BankAccount
    {
        private decimal balance; //Ammount in the account.
        private Currency currency; //Currency of Account.
        private decimal interest;

        /*I will have to change the constructor in order to better connect together with the BankTransfer class,
         * for the structure of that other class uses properties with uppercase letters instead of accessing properties with "this." the way that I have chosen to do.
         I must change my outgoing properties to Uppercase letters in order to better connect with the necessary class for conversion of currency.*/
        public SavingsAccount (decimal balance, Currency currency, decimal interest) : base ( )
        {
            this.balance = balance;
            this.currency = currency;
            this.interest = interest;
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
