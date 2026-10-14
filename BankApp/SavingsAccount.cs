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
    }
}
