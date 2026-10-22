using BankApp.Marias;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Robin
{
    internal class Menu
    {
        public List<User> users = new List<User> { new User(1, "0204", "123", "Robin@outlook.com", "076918", UserRole.Customer) };
        public void ShowCreatedUser()
        {
            foreach (var user in users)
            {
                Console.WriteLine($"\nDina uppgifter" +
                  $"\n{user.Username}" +
                  $"\n{user.Password}" +
                  $"\n{user.Email}" +
                  $"\n{user.PhoneNumber}" +
                  $"\n{user.Role}");
            }
        }
        public User CreateUser()
        {
            Console.Clear();
            Console.Write("\nVänligen ange uppgifter.");
            Console.Write("\nPersonnummer : ");
            string userName = Console.ReadLine() ?? "";
            Console.Write("\nLösenord : ");
            string userPass = Console.ReadLine() ?? "";
            Console.Write("\nEmail : ");
            string userEmail = Console.ReadLine() ?? "";
            Console.Write("\nTelefonnummer : ");
            string userPhoneNumber = Console.ReadLine() ?? "";

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
                        if (users.Count < 1)
                        {
                            Console.WriteLine("Fel");
                            userMenu = false;
                            break;
                        }
                        Console.Write("\nAnge personnummer :");
                        string userNumber = Console.ReadLine() ?? "";
                        Console.Write("\nAnge lösenord : ");
                        string userPassword = Console.ReadLine() ?? "";

                        foreach (var user in users)
                        {
                            if (userNumber == user.Username && userPassword == user.Password)
                            {
                                Console.WriteLine("Korrekt!");
                                ShowUserMenu();
                            }
                            else
                            {
                                Console.WriteLine("Fel!");
                                userMenu = false;
                            }
                        }
                        break;

                    case 2:

                        Console.Clear();
                        users.Add(CreateUser());
                        ShowCreatedUser();
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


