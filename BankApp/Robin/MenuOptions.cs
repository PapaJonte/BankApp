using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Robin
{
    //internal class MenuChoice
    //{
    //    public string MenuOption { get; set; }

    //    public MenuChoice(string menuOptions)
    //    {
    //        MenuOption = menuOptions;
    //    }
    //}

    //bool userMenu = true;

    //while (userMenu)
    //{
    //    Console.WriteLine("\n1. Logga in" +
    //        "\n2. Skapa konto" +
    //        "\n3. Avsluta");

    //    int.TryParse(Console.ReadLine(), out int userInput);

    //    switch (userInput)
    //    {
    //        case 1:
    //            Console.Clear();
    //            if (users.Count < 1)
    //            {
    //                Console.WriteLine("Fel");
    //                userMenu = false;
    //                break;
    //            }
    //            Console.Write("\nAnge personnummer :");
    //            string userNumber = Console.ReadLine() ?? "";
    //            Console.Write("\nAnge lösenord : ");
    //            string userPassword = Console.ReadLine() ?? "";

    //            foreach (var user in users)
    //            {
    //                if (userNumber == user.Username && userPassword == user.Password)
    //                {
    //                    Console.WriteLine("Korrekt!");
    //                    ShowUserMenu();
    //                }
    //                else
    //                {
    //                    Console.WriteLine("Fel!");
    //                    userMenu = false;
    //                }
    //            }
    //            break;

    //        case 2:

    //            Console.Clear();
    //            users.Add(CreateUser());
    //            ShowCreatedUser();
    //            break;

    //        case 3:
    //            Console.Clear();
    //            Console.WriteLine("Du valde att avsluta");
    //            userMenu = false;
    //            break;

    //        default:
    //            Console.Clear();
    //            Console.WriteLine("\nVänligen välj 1-3!");
    //            break;
    //    }
    //}
}
