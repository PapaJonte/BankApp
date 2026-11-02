using BankApp.Transaction_Layer.API;
using BankApp.Transaction_Layer.Enum;
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
        
        /*This method is intended to support the CreateAccount() method below.
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
        public void CreateAccount(int userId, int bankAccountNumber)
        {
            /*Creation of object of the BankAccount class to represent an account held by a user,
            rather than individual spaces to have money in.*/
            BankAccount userAccount = new BankAccount(userId, bankAccountNumber);

            BankAccountList.Add(userAccount);
            BankAccountDictionary.Add(userId, BankAccountList);
        }
        //Used for creating accounts.
        public void CreateSavingsAccount(int userId, int SavingsAccountNumber)
        {
            if (BankAccountList.Count > 0)
            {
                foreach (var account in BankAccountList)
                {
                    if (account.GetUserId() == userId)
                    {
                        //Interest for savings accounts.
                        decimal interest = 0.5m;

                        //Communication is needed before I adapt this one further.
                        Console.WriteLine(
                            "You get " + interest + " in interest.");
                        Console.WriteLine(
                            "Write how much money you want in the account.");

                        string stringInput = Console.ReadLine();
                        int convertedInput;

                        //Safety system against incorrect input.
                        if (Int32.TryParse(stringInput, out convertedInput))
                        {
                            SavingsAccount savingsAccount = new SavingsAccount(
                            convertedInput, CheckCurrency(), interest, SavingsAccountNumber);

                            account.SavingAccounts.Add(savingsAccount);
                        }
                        else
                        {
                            Console.WriteLine("Error! You must write a number.");
                        }
                    }
                }
            }
             
        }

        //Method for accessing the savingsaccounts.
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
                            Console.WriteLine(sAccount.GetBalance() + " " + sAccount.GetCurrency());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error! You have no accounts in the bank.");
                    }
                }
            }
        }

        //Puts balance of all savngs accounts together into a total value after converting them.
        public decimal GetTotalBalance(int userId)
        {
            decimal totalBalance = 0; //Where we put together all of the money into a total value.
            decimal additionValue = 0;

            foreach (var bAccount in BankAccountList)
            {
                //Just to check if there is anything in the list.
                if (BankAccountList.Count > 0)
                {
                    //Here we choose Swedish crowns as the currency.
                    Currency chosenValue = Currency.SEK;

                    ExchangeRate conversionTool = new ExchangeRate();
                    if (bAccount.GetUserId() == userId)
                    {
                        //Making sure the balance of all of the accounts are combined into a total balance.
                        foreach (var account in bAccount.SavingAccounts)
                        {
                            if (account.GetCurrency() != chosenValue)
                            {
                                //Don't be fooled, this is the same line of code.
                                conversionTool.TryCalculateExchangeValue(
                                    account.GetCurrency(),
                                    chosenValue, account.GetBalance(),
                                    out additionValue);

                                totalBalance = totalBalance + additionValue;
                            }
                            else
                            {
                                totalBalance = totalBalance + account.GetBalance();
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error! You have no accounts on the bank.");
                }
            }
            return totalBalance;
        }

        public void AccountOwnershipVerification(int userId, int bankAccountNr)
        {
            foreach( var account in BankAccountList)
            {
                if ( account.GetBankAccountNumber() == bankAccountNr && account.GetUserId() == userId)
                {
                    Console.WriteLine("Account found.");
                }
            }
            
        }
    }
}