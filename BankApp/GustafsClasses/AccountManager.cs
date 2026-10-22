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
        //We'll keep our savingsAccounts here.
        List<SavingsAccount> savingAccounts = new List<SavingsAccount>();

        //This method is intended to support the CreateAccount() method below. It will be called upon when CreateAccount needs it. Don't call upon this method!
        private Currency CheckCurrency()
        {
            //while loop around to make sure the user gives a correct value as currency.
            while ( true)
            {
                Console.WriteLine("Skriv vilken valuta du vill sätta i ditt sparkonto.");
                string currencyString = Console.ReadLine();

                Enum.GetValues<Currency>();

                if (Enum.TryParse(currencyString, true, out Currency parsedCurrency))
                {
                    foreach (var c in Enum.GetValues<Currency>())
                    {
                        if (c == parsedCurrency)
                        {
                            Console.WriteLine("Valutan är registrerad.");

                            return parsedCurrency;
                        }
                    }
                }
            }
        }
        //Used for creating accounts.
        public void CreateAccount()
        {
            //Interest for savings accounts.
            decimal interest = 0.5m;

            //Communication is needed before I adapt this one further.
            Console.WriteLine("Du får " + interest + " i ränta på kontot.");
            Console.WriteLine("Skriv hur mycket pengar som ska finnas i kontot.");

            string stringInput = Console.ReadLine();
            int convertedInput;

            //Safety system against incorrect input.
            if (Int32.TryParse(stringInput, out convertedInput))
            {
                SavingsAccount savingsAccount = new SavingsAccount(convertedInput, CheckCurrency(), interest);

                //Adding the newly created account to the list.
                savingAccounts.Add(savingsAccount);
            }
            else
            {
                Console.WriteLine("Error! Du måste skriva en siffra.");
            }
        }

        //Method for accessing the savingsaccounts.
        public void GetAccounts()
        {
            //if statement as a safety measure to assure ourselves that the list is not null.
            if (savingAccounts.Count > 0 )
            {
                foreach (var account in savingAccounts)
                {
                    //Displays balance and currency of account.
                    Console.WriteLine(account.GetBalance() + " " + account.GetCurrency());
                }
            }
            else
            {
                Console.WriteLine("Error! Du har inga konton på banken.");
            }
        }
    }
}