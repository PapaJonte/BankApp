using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    //Upper class of SavingsAccount
    internal class BankAccount
    {
        /*Put this list inside of the BankAccount class,
        so that each Bank account has it's own individual list of savingsAccounts.*/
        public List<SavingsAccount> savingAccounts = new List<SavingsAccount>();
        public Dictionary<int, List<SavingsAccount> > savingsAccountDictionary = new Dictionary<int, List<SavingsAccount>>();
        private int userId;

        /*Constructor making sure that we have applied an id to the account,
        to distinguish it from others.*/
        public BankAccount(int userId )
        {
            this.userId = userId;
        }

        public int GetUserId()
        {
            return userId;
        }
    }
}
