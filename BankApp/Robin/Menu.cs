using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Robin
{
    internal class Menu
    {
        public void ShowMainMenu()
        {
            Console.WriteLine("Välkommen till banken!" +
                "\n1. Logga in" +
                "\n2. Visa konton" +
                "\n3. Överför pengar" +
                "\n4. Avsluta.");

            int.TryParse(Console.ReadLine(), out int userInput);
            //while (!int.TryParse(), out userInput))
            {
                Console.Clear();
                Console.WriteLine("\nVänligen välj 1-4");
                Console.WriteLine("\nVälkommen till banken!" +
               "\n1. Logga in" +
               "\n2. Visa konton" +
               "\n3. Överför pengar" +
               "\n4. Avsluta.");
            }
            //bool userMenu = true;

            switch (userInput)
            {
                case 1:
                    Console.Clear();
                    Console.WriteLine("Ange personnummer :");
                    int.TryParse(Console.ReadLine(), out int userNumber);
                    Console.WriteLine(userNumber);
                    Console.WriteLine("Ange lösenord : ");
                    string userPassword = Console.ReadLine();
                    break;

                case 2:

                    break;

                case 3:

                    break;

                case 4:

                    break;

                default:

                    break;
            }
        }
    }
}
