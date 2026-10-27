using BankApp.Jonathan;
using BankApp.Robin;

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

            menu.LoginMenu();

            LoanTests.RunTests();
        }
    }
}
