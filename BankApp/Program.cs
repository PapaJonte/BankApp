using BankApp.Jonathan;
using BankApp.Robin;

namespace BankApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UserService userService = new UserService();
            Menu menu = new Menu(userService);

            menu.LoginMenu();

            LoanTests.RunTests();
        }
    }
}
