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
            UserService userService = new UserService();
            AuthService authService = new AuthService(userService);
            Menu menu = new Menu(userService, authService);

            AnsiConsole.MarkupLine("[red bold]Hello World[/]");
            AnsiConsole.MarkupLine("[slowblink]Hello World[/]");

            Console.ReadKey();

            //menu.LoginMenu();

            LoanTests.RunTests();
        }
    }
}
