using BankApp.Jonathan;
using BankApp.Pull_The_Sack;
using BankApp.Robin;
using BankApp.Transaction_Layer.API;
using BankApp.Transaction_Layer.Controller;
using BankApp.Transaction_Layer.Data;
using BankApp.Transaction_Layer.Service;
using BankApp.GustafsClasses;
using Spectre.Console;
using BankApp.GustafsClasses;

namespace BankApp
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Theme theme = new Theme();
            UserService userService = new UserService();
            AuthService authService = new AuthService(userService);
            CreditScoreService creditScoreService = new CreditScoreService();
            InterestRuleService interestRuleService = new InterestRuleService();
            LoanService loanService = new LoanService(interestRuleService, creditScoreService);
            AccountManager accountManager = new AccountManager();
            LoanMenu loanMenu = new LoanMenu(loanService, accountManager); //This object appears to demand an object of BankTransfer as an additional parameter.

            #region Transactions
            AccountBridge accountBridge = new(accountManager);
            #region  ExchangeRate Module
            ExchangeRate exchangeRateModule = new();
            #endregion
            #region Transaction Module
            TransactionScheduler transactionScheduler = new();
            TransactionRegistry transactionRegistry = new();
            StageTransaction transactionStaging = new(transactionScheduler, transactionRegistry, accountBridge);
            BankTransaction bankTransactionModule = new(transactionStaging, exchangeRateModule, accountBridge);
            BankTransfer bankTransferModule = new(bankTransactionModule);
            #endregion
            LoanMenu loanMenu = new LoanMenu(loanService, accountManager,bankTransferModule);

            #region Transfer Module
            #endregion
            #endregion

            Menu menu = new Menu(userService, authService, loanMenu);


            // Console.ReadKey();

            menu.LoginMenu();
        }
    }
}
