using BankApp.Jonathan;
using BankApp.Robin;

namespace BankApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Menu menu = new Menu();

            menu.ShowMainMenu();

            LoanTests.RunTests();
        }
    }
}
