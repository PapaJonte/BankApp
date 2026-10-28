using BankApp.Jonathan;
using BankApp.Robin;
using Spectre.Console;

namespace BankApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //User user = new User();
            
            // SERVICES
            UserService userService = new UserService();
            AuthService authService = new AuthService(userService);
            InterestRuleService interestRuleService = new InterestRuleService();
            CreditScoreService creditScoreService = new CreditScoreService();
            LoanService loanService = new LoanService(interestRuleService, creditScoreService);

            Menu menu = new Menu(userService, authService);

            AnsiConsole.MarkupLine("[red bold]Hello World[/]");
            AnsiConsole.MarkupLine("[slowblink]Hello World[/]");

            Console.ReadKey();

            //menu.LoginMenu();

            LoanTests.RunTests();
        }
    }
}
