
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
        public void ShowAdminMenu()
        {

            Console.Clear();
            bool userMenu = true;

            int selectedIndex = 0;

            while (userMenu)
            {
                Console.WriteLine("\n Skapa nya användare" +
                    "\nLån" +
                    "\nVäxelkurser" +
                    "\nGå tillbaka till startsidan");
                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + 4) % 4;

                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % 4;
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedIndex)
                        {
                            case 0:
                                CreateUser();
                                break;

                            case 1:
                                // Acceptera eller avböja lån
                                break;

                            case 2:
                                // Växelkurser
                                break;

                            case 3:
                                userMenu = false;
                                break;
                        }
                        break;

                    case ConsoleKey.Escape:
                        userMenu = false;

                        break;
                }
            }
        }

        //public void 
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

            int selectedIndex = 0;

            while (userMenu)
            {
                Console.WriteLine("\n Se Konton" +
                    "Gå tillbaka till startsida");

                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + 4) % 4;

                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % 4;
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedIndex)
                        {
                            case 0:
                                // Skapa användare
                                break;

                            case 1:
                                // Acceptera eller avböja lån
                                break;

                            case 2:
                                // Växelkurser
                                break;

                            case 3:
                                userMenu = false;
                                break;
                        }
                        break;

                    case ConsoleKey.Escape:
                        userMenu = false;

                        break;
                }
            }
            //Console.Clear();
            //bool userMenu = true;

            //while (userMenu)
            //{
            //    Console.WriteLine("\n1. Se konton" +
            //        "\n2. Gå tillbaka till startsida");
            //    int.TryParse(Console.ReadLine(), out int userInput);

            //    switch (userInput)
            //    {
            //        case 1:
            //            Console.Clear();
            //            ShowCreatedUser();
            //            break;

            //        case 2:
            //            Console.Clear();
            //            userMenu = false;
            //            break;

            //        default:
            //            Console.Clear();
            //            Console.WriteLine("\nVänligen välj 1-2!");
            //            break;
            //    }
            //}
        }
        public void LoginMenu()
        {
            Console.Clear();
            bool userMenu = true;

            int selectedIndex = 0;

            while (userMenu)
            {
                Console.Clear();
                Console.WriteLine("\nLogga in" +
                    "\nSkapa konto" +
                    "\nAvsluta");

                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + 4) % 4;

                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % 4;
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedIndex)
                        {
                            case 0:
                                // Skapa användare
                                break;

                            case 1:
                                // Acceptera eller avböja lån
                                break;

                            case 2:
                                // Växelkurser
                                break;

                            case 3:
                                userMenu = false;
                                break;
                        }
                        break;

                    case ConsoleKey.Escape:
                        userMenu = false;

                        break;
                }
            }
        }
    }
}


