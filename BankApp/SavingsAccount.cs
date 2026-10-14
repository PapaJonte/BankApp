using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    
    /*I have decided to make this class directly below the AccountManager class in it's relationship, but I will change it I'm told. I'm not sure about the structure.*/
    internal class SavingsAccount : AccountManager
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
