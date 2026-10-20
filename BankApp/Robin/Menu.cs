using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Robin
{
    internal class Menu
    {
        public void ShowMainMenu()
        {
            MenuChoice menuOptions = new MenuChoice("1. Logga in");
            MenuChoice menuOptions2 = new MenuChoice("2. Skapa ett konto");
            MenuChoice menuOptions3 = new MenuChoice("3. Avsluta");

            List<MenuChoice> menus = new List<MenuChoice>();
            menus.Add(menuOptions);
            menus.Add(menuOptions2);
            menus.Add(menuOptions3);

            bool userMenu = true;

            while (userMenu)
            {
                foreach (var m in menus)
                {
                    Console.WriteLine(m.MenuOption);
                }

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
                        break;

                    case 2:
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


