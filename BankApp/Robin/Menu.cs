using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Robin
{
    internal class Menu
    {
        User? user = null;

        public void ShowCreatedUser()
        {
              Console.WriteLine($"\nDina uppgifter" +
                $"\n{user.Username}" +
                $"\n{user.Password}" +
                $"\n{user.Email}" +
                $"\n{user.PhoneNumber}" +
                $"\n{user.Role}");
        }
        public User CreateUser()
        {
            Console.Clear();
            Console.Write("\nVänligen ange uppgifter.");
            Console.Write("\nPersonnummer : ");
            string userName = Console.ReadLine();
            Console.Write("\nLösenord : ");
            string userPass = Console.ReadLine();
            Console.Write("\nEmail : ");
            string userEmail = Console.ReadLine();
            Console.Write("\nTelefonnummer : ");
            string userPhoneNumber = Console.ReadLine();

            User user = new User(1, userName, userPass, userEmail, userPhoneNumber, UserRole.Customer);
            Console.WriteLine($"\nDu skapade ett konto!" +
                $"\nDina uppgifter." +
                $"\n{user.Username}" +
                $"\n{user.Password}" +
                $"\n{user.Email}" +
                $"\n{user.PhoneNumber}" +
                $"\n{user.Role}");



            return user;
        }

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
                        ShowCreatedUser();
                        break;

                    case 2:
                        Console.Clear();
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
                        Console.Write("\nAnge personnummer :");
                        string userNumber = Console.ReadLine();
                        if(userNumber == user.Username)
                        {
                            Console.WriteLine("Korrekt");
                        }
                        else
                        {
                            Console.WriteLine("Fel");
                            userMenu = false;
                        }
                        Console.Write(userNumber);
                        Console.Write("\nAnge lösenord : ");
                        string userPassword = Console.ReadLine();
                        if(userPassword == user.Password)
                        {
                            Console.WriteLine("Korrekt!");
                        }
                        else
                        {
                            Console.WriteLine("Fel");
                            userMenu = false;
                        }
                        ShowUserMenu();
                        break;

                    case 2:

                        Console.Clear();
                        user = CreateUser();
                        ShowCreatedUser();

                        //Fråga användaren om den vill bekräfta kontot.
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


