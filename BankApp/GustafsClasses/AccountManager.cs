using BankApp.Transaction_Layer.API;
using BankApp.Transaction_Layer.Enum;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    /*This class is the upper class of BankAccount and SavingsAccount.*/
    internal class AccountManager
    {
        List<BankAccount> bankAccountList = new List<BankAccount>();

        Dictionary<int, bankAccountList> BankAccountDictionary = 
        new Dictionary<int, bankAccountList>();

        BankAccountDictionary.Add(bankAccountList);

        //This method is intended to support the CreateAccount() method below. It will be called upon when CreateAccount needs it. Don't call upon this method!
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
        public void CreateAccount(int userId)
        {
            /*Creation of object of the BankAccount class to represent an account held by a user,
            rather than individual spaces to have money in*/
            BankAccount userAccount = new BankAccount(userId);

            bankAccountList.Add(userAccount);
        }
        //Used for creating accounts.
        public void CreateSavingsAccount(int userId)
        {
            foreach (var account in bankAccountList)
            {
                if( account.userId == userId)
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
                        convertedInput, CheckCurrency(), interest);

                        account.savingAccounts.Add(savingsAccount);
                    }
                    else
                    {
                        Console.WriteLine("Error! You must write a number.");
                    }
                }
            } 
        }

        //Method for accessing the savingsaccounts.
        public void GetAccounts( int userId)
        {
            //if statement as a safety measure to assure ourselves that the list is not null.
            if (savingAccounts.Count > 0 )
            {
                foreach (var account in savingAccounts)
                {
                    //Displays balance and currency of account.
                    Console.WriteLine(
                        account.GetBalance()
                        + " " + account.GetCurrency());
                }
            }
            else
            {
                Console.WriteLine(
                    "Error!
                    You have no accounts in the bank.");
            }
        }

        //Puts balance of all savngs accounts together into a total value after converting them.
        public decimal GetTotalBalance( userId)
        {
            decimal totalBalance = 0; //Where we put together all of the money into a total value.
            decimal additionValue = 0;

            if (savingAccounts.Count > 0)
            {
                //Here we decide which currency to convert to.
                Currency chosenValue = CheckCurrency();

                ExchangeRate conversionTool = new ExchangeRate();

                //Making sure the balance of all of the accounts are combined into a total balance.
                foreach (var account in savingAccounts)
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
            else if (savingAccounts.Count == 1)
            {
                Console.WriteLine("You only have one savings account.");
            }

            else
            {
                Console.WriteLine("Error! You have no savings accounts on the bank.");
            }
            
            return totalBalance;
        }
    }
}