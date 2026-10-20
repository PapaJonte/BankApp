using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    
    /*The place in the class hierarchy has been changed so that SavingsAccount is now a lower class of BankAccount.*/
    internal class SavingsAccount : BankAccount
    {
        //Properties.
        private decimal balance; //Ammount in the account.
        private decimal interest;

        //The constructor does no longer serve as clear of a purpose as before. Still I'll keep it for now.
        public SavingsAccount (decimal balance, decimal interest) : base ( )
        {
            this.balance = balance;
            this.interest = interest;
        }

        //Method that returns account balance.
        public decimal GetBalance()
        {
            return balance;
        }
        //Method that returns interest.
        public decimal GetInterest()
        {
            return interest;
        }
    }
}
