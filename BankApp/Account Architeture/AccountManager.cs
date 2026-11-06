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
        private readonly ExchangeRate conversionTool;

        public AccountManager(ExchangeRate conversionTool)
        {
            this.conversionTool = conversionTool ?? throw new ArgumentNullException(nameof(conversionTool));
        }
        /*This method is intended to support the CreateAccount() method below by selecting and identifying currency.
         * It will be called upon when CreateAccount needs it.
         * Don't call upon this method!*/
        public static Currency CheckCurrency()
        {

            int index = 0;
            foreach (var e in System.Enum.GetValues<Currency>())
            {
                ++index;
                Console.Write($"{e}, ");

                if (index % 10 == 0)
                    Console.WriteLine("");
            }
            Console.WriteLine("");
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
        public void CreateAccount(int userId, decimal balance, Currency currency)
        {
            
            /*Creation of object of the BankAccount class to represent an account held by a user,
            that may both contain balance, and savingsAccounts.*/
            BankAccount userAccount = new BankAccount(userId, AccountNrGenerator(),
                balance, currency);



            BankAccountList.Add(userAccount);
            if (!BankAccountDictionary.TryGetValue(userId, out var customerAccounts))
            {
                customerAccounts = new List<BankAccount>();
                BankAccountDictionary.Add(userId, customerAccounts);
            }

            customerAccounts.Add(userAccount);
        }
        //Used for creating savings accounts. The savings accounts store the balance.
        public SavingsAccount? CreateSavingsAccount(int userId, int bankAccountNumber, Currency currency)
        {
            //Variables for adding values to the object's constructor.
            decimal balance = 0m;
            decimal interest = 0.5m;
            //Attempting to find object of BankAccount class.
            BankAccount? parentAccount = FindRegularAccount(userId, bankAccountNumber);
            //Returning nothing if there is nothing to be found.
            if (parentAccount == null)
            {
                return null;
            }
            //Creating an object of SavingsAccount class, as is the point of this method.
            SavingsAccount savingsAccount = new SavingsAccount(
                    balance, currency, interest, AccountNrGenerator() );

            parentAccount.SavingAccounts.Add(savingsAccount); //Adding account to list to save for later.
            //Saving the list for later.
            parentAccount.SavingsAccountDictionary[userId] = parentAccount.SavingAccounts;
            return savingsAccount;
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
        private decimal ConvertBalanceToSek(decimal balance, Currency currency)
        {
            if (currency == Currency.SEK || balance == 0m)
            {
                return balance;
            }

            StatusCode status = conversionTool.TryCalculateExchangeValue(
                currency, Currency.SEK, balance, out decimal convertedBalance);

            if (status != StatusCode.Successful)
            {
                throw new InvalidOperationException(
                    $"No exchange rate is available from {currency} to SEK.");
            }

            return convertedBalance;
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