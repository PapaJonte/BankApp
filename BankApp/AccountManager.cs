using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    /*This class is the upper class of BankAccount and SavingsAccount. Changed into abstract since this class is for inheritance purposes.
     Do correct me if I'm wrong.*/
    internal abstract class AccountManager
    {
        //Properties for inheritance by lower classes.
        private string passWord;
        private string emailAddress;

        private int phoneNumber;
        private int bankID;
    }
}
