using BankApp.Jonathan;
using BankApp.Pull_The_Sack;
using BankApp.Robin;

namespace BankApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Menu menu = new Menu();

            // menu.LoginMenu();

            // LoanTests.RunTests();

            Orchestrator orc = new();
            orc.Start(100000);
        }
    }
}
