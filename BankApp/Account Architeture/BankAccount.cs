using BankApp.Transaction_Layer.Enum;
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
        //Ammount in the account.
        private decimal balance; 
        //Currency of Account.
        private Currency currency; 
        

        /*Constructor making sure that we have applied an id to the account,
        to distinguish it from others.*/
        public BankAccount(int userId, int bankAccountNumber, decimal balance, Currency currency)
        {
            this.userId = userId;
            this.bankAccountNumber = bankAccountNumber;

            this.balance = balance;
            this.currency = currency;
        }

        public int GetUserId()
        {
            return userId;
        }

        public int GetBankAccountNumber()
        {
            return bankAccountNumber;
        }

        //For transfer of balance. Bool determines if you add or remove balance.
        public void TransferBalance(decimal transferSum, bool addMore)
        {
            if (addMore == true)
            {
                balance = balance + transferSum;
            }
            else
            {
                balance = balance - transferSum;
            }
        }
        //Method that notifies of threshold being met at transfer.
        public int GetThresholdNotification(decimal transferSum, TransactionType transactionType)
        {
            int transferThreshold;
            Enum.GetValues<TransactionType>();

            if (transactionType == TransactionType.Deposit)
            {
                transferThreshold = 10000;
                return transferThreshold;
            }
            else
            {
                transferThreshold = -10000;
                return transferThreshold;
            }
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
    }
}
