using BankApp.Jonathan;

namespace BankApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            AccountManager manager = new AccountManager();

            manager.CreateAccount();
            manager.CreateAccount();
            manager.CreateAccount();

            manager.GetAccounts();
        }
    }
}
