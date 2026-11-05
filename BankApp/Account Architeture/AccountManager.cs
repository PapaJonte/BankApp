using BankApp.Transaction_Layer.API;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transaction_Layer.Service;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace BankApp.GustafsClasses
{
    /*This class is the upper class of BankAccount and SavingsAccount.*/
    internal class AccountManager
    {
        public List<BankAccount> BankAccountList = new List<BankAccount>();

        public Dictionary<int, List<BankAccount>> BankAccountDictionary = new Dictionary<int, List<BankAccount>>();
        
        /*This method is intended to support the CreateAccount() method below by selecting and identifying currency.
         * It will be called upon when CreateAccount needs it.
         * Don't call upon this method!*/
        public static Currency CheckCurrency()
        {
            //while loop around to make sure the user gives a correct value as currency.
            while ( true)
            {
                Console.WriteLine(
                    "Write which currency you want in your account.");
                string currencyString = Console.ReadLine();

                Enum.GetValues<Currency>();

                if (Enum.TryParse(currencyString, true, out Currency parsedCurrency))
                {
                    foreach (var c in Enum.GetValues<Currency>())
                    {
                        if (c == parsedCurrency)
                        {
                            Console.WriteLine("Currency is registered.");

                            return parsedCurrency;
                        }
                    }
                }
            }
        }
        //Generates random number for Account numbers of both savings accounts and bank accounts.
        private int AccountNrGenerator()
        {
            
            int accountNumber = UniqueIDGenerator.GenerateUniqueID();
            return accountNumber;
        }
        //This method will create a bank account. Not to be confused with a savings account. Bank accounts store the savings accounts.
        public void CreateAccount(int userId, decimal balance)
        {
            
            /*Creation of object of the BankAccount class to represent an account held by a user,
            that may both contain balance, and savingsAccounts.*/
            BankAccount userAccount = new BankAccount(userId, AccountNrGenerator(),
                balance, Currency.SEK);

            BankAccountList.Add(userAccount);
            if (!BankAccountDictionary.TryGetValue(userId, out var customerAccounts))
            {
                customerAccounts = new List<BankAccount>();
                BankAccountDictionary.Add(userId, customerAccounts);
            }

            customerAccounts.Add(userAccount);
        }
        //Used for creating savings accounts. The savings accounts store the balance.
        public decimal CreateSavingsAccount(int userId, decimal balance)
        {
            Enum.GetValues<Currency>();
            //Return data.
            decimal interest = 0.5m;
            if (BankAccountList.Count > 0)
            {
                foreach (var account in BankAccountList)
                {
                    if (account.GetUserId() == userId)
                    {
                        SavingsAccount savingsAccount = new SavingsAccount(
                            balance, Currency.SEK, interest, AccountNrGenerator());

                        account.SavingAccounts.Add(savingsAccount);
                        if (account.SavingsAccountDictionary.Count == 0)
                        {
                            account.SavingsAccountDictionary.Add(userId, account.SavingAccounts);
                        }
                    }
                }
            }
            return interest;
        }

        /*Method for accessing the savingsaccounts.
         * Method does not return anything.
         * It merely allows you to see the balance and currency of savingsAccounts.*/
        public void GetAccounts( int userId)
        {
            if (BankAccountList.Count != 0)
            {
                foreach (var bAccount in BankAccountList)
                {
                    if (bAccount.GetUserId() == userId)
                    {
                        Console.WriteLine(bAccount.GetBalance() + " " + bAccount.GetCurrency());

                        if (bAccount.SavingAccounts.Count > 0)
                        {
                            foreach (var sAccount in bAccount.SavingAccounts)
                            {
                                //Displays balance and currency of account.
                                Console.WriteLine(
                                    sAccount.GetBalance() + " " + sAccount.GetCurrency()
                                    );
                            }
                        }
                        else
                        {
                            Console.WriteLine("You have no savings accounts in the bank.");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Error! You have no accounts in the bank.");
            }
        }

        //Puts balance of all savngs accounts of a user together into a total value after converting them to SEK.
        public decimal GetTotalBalance(int userId)
        {
            decimal totalBalance = 0m;

            foreach (BankAccount account in BankAccountList)
            {
                if (account.GetUserId() != userId)
                {
                    continue;
                }

                totalBalance += ConvertBalanceToSek(account.GetBalance(), account.GetCurrency());

                foreach (SavingsAccount savings in account.SavingAccounts)
                {
                    totalBalance += ConvertBalanceToSek(savings.GetBalance(), savings.GetCurrency());
                }
            }

            return totalBalance;
        }
        public BankAccount? FindRegularAccount(int userId, int accountNumber)
        {
            return BankAccountList.Find(account =>
                account.GetUserId() == userId
                && account.GetBankAccountNumber() == accountNumber);
        }
        //Verifies ownership of Bank Account by bank account number and user ID. Will return a bool value depending on result.
        public bool AccountOwnershipVerification(int userId, int bankAccountNr)
        {
            foreach( var account in BankAccountList)
            {
                if ( account.GetBankAccountNumber() == bankAccountNr && account.GetUserId() == userId)
                {
                    Console.WriteLine("Account found.");
                    return true;
                }
            }
            return false;
        }
        
    }
}