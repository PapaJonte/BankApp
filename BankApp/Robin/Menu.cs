using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Robin
{
    internal class Menu
    {
        public void ShowUserMenu()
        {
            Console.Clear();
            bool userMenu = true;

            while (userMenu)
            {
                Console.WriteLine("\n1. Se konton" +
                    "\n2. Gå tillbaka till startsida");
                int.TryParse(Console.ReadLine(), out int userInput);

                switch (userInput)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("Lista");
                        break;

                    case 2:
                        Console.Clear();
                        //Console.WriteLine("Gå tillbaka till startsida");
                        userMenu = false;
                        break;

                    default:
                        Console.Clear();
                        Console.WriteLine("\nVänligen välj 1-2!");
                        break;
                }
            }
        }
        public void ShowMainMenu()
        {
            bool userMenu = true;

            while (userMenu)
            {

                Console.WriteLine("\n1. Logga in" +
                    "\n2. Skapa konto" +
                    "\n3. Avsluta");

                int.TryParse(Console.ReadLine(), out int userInput);

                switch (userInput)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("Ange personnummer :");
                        string userNumber = Console.ReadLine();
                        Console.WriteLine(userNumber);
                        Console.WriteLine("Ange lösenord : ");
                        string userPassword = Console.ReadLine();
                        ShowUserMenu();
                        break;

                    case 2:
                        Console.Clear();
                        Console.WriteLine("Case 2");
                        break;

                    case 3:
                        Console.Clear();
                        Console.WriteLine("Du valde att avsluta");
                        userMenu = false;
                        break;

                    default:
                        Console.Clear();
                        Console.WriteLine("\nVänligen välj 1-3!");
                        break;
                }
            }
        }
    }
}


