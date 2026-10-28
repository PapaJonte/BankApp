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
            Menu menu = new Menu(userService, authService);
            

            Console.ReadKey();

            //menu.LoginMenu();

            LoanTests.RunTests();
        }
    }
}
