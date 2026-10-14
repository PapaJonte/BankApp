using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    
    /*This class stores properties related to balance of the account.
     * I changed it's place in the hierarchy to be a lower class to BankAcount rather than AccountManager,
     * because I believe that I only made it a direct lower class out of a misunderstanding. Still not sure though.*/
    internal class SavingsAccount : BankAccount
    {
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
