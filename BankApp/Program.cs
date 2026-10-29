using BankApp.Jonathan;
using BankApp.Robin;
using Spectre.Console;

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
            LoanMenu loanMenu = new LoanMenu(loanService, accountManager);

            Menu menu = new Menu(userService, authService, loanMenu);
            

            Console.ReadKey();

            menu.LoginMenu();
        }
    }
}
