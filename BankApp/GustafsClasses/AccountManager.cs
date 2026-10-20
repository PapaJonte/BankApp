using BankApp.Transaction_Layer.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    /*This class is the upper class of BankAccount and SavingsAccount. Changed into abstract since this class is for inheritance purposes.
     Do correct me if I'm wrong.*/
    internal abstract class AccountManager
    {
        //We'll keep our savingsAccounts here.
        List<SavingsAccount> savingAccounts = new List<SavingsAccount>();

        public void CheckCurrency()
        {
            Console.WriteLine("Skriv vilken valuta du vill sätta i ditt sparkonto.");
            string currencyString = Console.ReadLine();

            if(Enum.TryParse(currencyString, true, out Currency parsedCurrency))
            {
                foreach(var c in Enum.GetValues<Currency>())
                {
                    if(c == parsedCurrency)
                    {
                        Console.WriteLine("Valutan är registrerad.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Error! Valutan hittades inte. Väljer SEK som standardval.");
            }
        }

        public void CreateAccount()
        {
            //This is a placeholder value until we have properly connected eachothers code.
            decimal interest = 4.1m;

            //Communication is needed before I adapt this one further.
            Console.WriteLine("Du får " + interest + " i ränta på kontot.");
            Console.WriteLine("Skriv hur mycket pengar som ska finnas i kontot.");

            string stringInput = Console.ReadLine();
            int convertedInput;
            //Safety system against incorrect input.
            if (Int32.TryParse(stringInput, out convertedInput))
            {
                SavingsAccount savingsAccount = new SavingsAccount(convertedInput, interest);
                savingAccounts.Add(savingsAccount);
            }
            else
            {
                Console.WriteLine("Error! Du måste skriva en siffra.");
            }
        }
    }
}