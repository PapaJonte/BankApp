using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    
    /*The place in the class hierarchy has been changed so that SavingsAccount is now a lower class of BankAccount.*/
    internal class SavingsAccount : BankAccount
    {
        //Properties.
        private double balance; //Ammount in the account.
        private float interest;

        //Constructor of class to assure ourselves that we're filling the variables with something before we use the methods.
        public SavingsAccount (string passWord, string emailAddress, int phoneNumber, int bankID, double balance, float interest) : base (passWord, emailAddress, phoneNumber, bankID)
        {
            this.balance = balance;
            this.interest = interest;
        }

        //Method that returns account balance.
        public double DisplayBalance()
        {
            return balance;
        }
        //Method that returns interest.
        public double DisplayInterest()
        {
            return interest;
        }
    }
}
