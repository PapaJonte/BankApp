using BankApp.Jonathan;
using BankApp.Pull_The_Sack;
using BankApp.Robin;
using BankApp.Transaction_Layer.Controller;
using BankApp.Transaction_Layer.Service;
using BankApp.GustafsClasses;
using Spectre.Console;
using BankApp.Pull_The_Sack.API;
using BankApp.Transactions.Transfers;
using BankApp.Transactions.Processing;
using BankApp.Transactions.Statistics;
using BankApp.Currencies;
using BankApp.ConsoleUi.Menus;

namespace BankApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region Accounts and users

            // Share one exchange-rate service across accounts, transactions and menus
            ExchangeRate exchangeRateModule = new();
            AccountManager accountManager = new AccountManager(exchangeRateModule);
            UserService userService = new UserService(accountManager);
            AuthService authService = new AuthService(userService);

            #endregion

            #region Loan services

            CreditScoreService creditScoreService = new CreditScoreService();
            InterestRuleService interestRuleService = new InterestRuleService();
            LoanService loanService = new LoanService(interestRuleService, creditScoreService);

            #endregion

            #region Transaction processing

            AccountBridge accountBridge = new(accountManager, loanService);
            TransactionScheduler transactionScheduler = new();
            TransactionRegistry transactionRegistry = new();
            StageTransaction transactionStaging = new(transactionScheduler, transactionRegistry, accountBridge);

            BankTransaction bankTransactionModule = new(transactionStaging, exchangeRateModule, accountBridge);
            BankTransfer bankTransferModule = new(bankTransactionModule);

            #endregion

            #region Transaction history and statistics

            TransactionStatisticFilter transactionFilter = new(transactionStaging);
            BankTransferStatistic bankTransferStatistic = new(transactionFilter);
            IViewTransactionStatisticBridge viewTransactionStatisticBridge = new TransactionStatisticBridge(bankTransferStatistic);

            #endregion

            #region Console menus

            Theme theme = new Theme();
            LoanMenu loanMenu = new LoanMenu(loanService, accountManager, bankTransferModule, exchangeRateModule);
            AdminLoanMenu adminLoanMenu = new AdminLoanMenu(loanService, accountManager, bankTransferModule);

            Menu menu = new Menu(userService, authService, loanMenu, adminLoanMenu, accountManager, bankTransferModule, bankTransferStatistic, exchangeRateModule, loanService);

            #endregion

            menu.LoginMenu();
        }
    }
}
