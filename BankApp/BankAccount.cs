using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    //This class has more to do with the account itself on the bank than the bank balance.
    internal class BankAccount : AccountManager
    {
        private string stringInput;
        private int convertedInput;

        //Constructor
        public BankAccount(string passWord, string emailAddress, int phoneNumber, int bankID, string stringInput, int convertedInput) : base(passWord, emailAddress, phoneNumber, bankID)
        {
            this.stringInput = stringInput;
            this.convertedInput = convertedInput;
        }
        //Log in method here.

        //Converter Method here

    }
}
