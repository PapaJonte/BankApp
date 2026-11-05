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
        private Currency CheckCurrency()
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
        public decimal CreateAccount(int userId, decimal balance)
        {
            decimal interest = 0.5m;
            /*Creation of object of the BankAccount class to represent an account held by a user,
            that may both contain balance, and savingsAccounts.*/
            BankAccount userAccount = new BankAccount(userId, AccountNrGenerator(),
                balance, interest, Currency.SEK);

            BankAccountList.Add(userAccount);
            BankAccountDictionary.Add(userId, BankAccountList);
            return interest;
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
            foreach (var bAccount in BankAccountList)
            {
                if (bAccount.GetUserId() == userId)
                {
                    if (bAccount.SavingAccounts.Count > 0 )
                    {
                        foreach (var sAccount in bAccount.SavingAccounts)
                        {
                            //Displays balance and currency of account.
                            Console.WriteLine(sAccount.GetBalance() + " " + sAccount.GetCurrency() );
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error! You have no accounts in the bank.");
                    }
                }
            }
        }

        //Puts balance of all savngs accounts of a user together into a total value after converting them to SEK.
        public decimal GetTotalBalance(int userId)
        {
            decimal totalBalance = 0; //Where we put together all of the money into a total value.
            decimal additionValue = 0;
            ExchangeRate conversionTool = new ExchangeRate();
            //Just to check if there is anything in the list.
            if (BankAccountList.Count > 0)
            {
                foreach (var bAccount in BankAccountList)
                {
                    //Here we choose Swedish crowns as the currency.
                    Currency chosenValue = Currency.SEK;

                    if (bAccount.GetUserId() == userId)
                    {
                        if (bAccount.GetCurrency() != chosenValue)
                        {
                            //Don't be fooled, this is the same line of code.
                            conversionTool.TryCalculateExchangeValue(
                                bAccount.GetCurrency(),
                                chosenValue, bAccount.GetBalance(),
                                out additionValue);

                            totalBalance = totalBalance + additionValue;
                        }
                        else
                        {
                            totalBalance = totalBalance + bAccount.GetBalance();
                        }
                    }
                    foreach (var sAccount in bAccount.SavingAccounts)
                    {
                        if (sAccount.GetCurrency() != chosenValue)
                        {
                            //Don't be fooled, this is the same line of code.
                            conversionTool.TryCalculateExchangeValue(
                                sAccount.GetCurrency(),
                                chosenValue, sAccount.GetBalance(),
                                out additionValue);

                            totalBalance = totalBalance + additionValue;
                        }
                        else
                        {
                            totalBalance = totalBalance + bAccount.GetBalance();
                        }
                    }
                } 
            }
            else
            {
                Console.WriteLine("Error! You have no accounts on the bank.");
            }
            
            return totalBalance;
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