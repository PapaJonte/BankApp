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
            InterestRuleService interestRuleService = new InterestRuleService();
            CreditScoreService creditScoreService = new CreditScoreService();
            LoanService loanService = new LoanService(interestRuleService, creditScoreService);

            Menu menu = new Menu(userService, authService);
            

            Console.ReadKey();

            menu.LoginMenu();
        }
    }
}
