using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.GustafsClasses
{
    //Upper class of SavingsAccount
    internal class BankAccount
    {
        /*Put this list inside of the BankAccount class,
        so that each Bank account has it's own individual list of savingsAccounts.*/
        public List<SavingsAccount> SavingAccounts = new List<SavingsAccount>();
        public Dictionary<int, List<SavingsAccount> > SavingsAccountDictionary = new Dictionary<int, List<SavingsAccount>>();

        private int userId;
        private int bankAccountNumber;

        /*Constructor making sure that we have applied an id to the account,
        to distinguish it from others.*/
        public BankAccount(int userId , int bankAccountNumber)
        {
            this.userId = userId;
            this.bankAccountNumber = bankAccountNumber;
        }

        public int GetUserId()
        {
            return userId;
        }

        public int GetBankAccountNumber()
        {
            return bankAccountNumber;
        }
    }
}
