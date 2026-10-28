
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Robin
{
    internal class Menu
    {

        // Skapar privata fält av userservice och authservice
        private readonly UserService _userService;
        private readonly AuthService _authService;
        // CONSTRUCTOR INJECTION: Vi kräver att UserService skickas in när Menu skapas

        // Skickar in båda fälten i vår constructor för att kunna använda metoderna från marias klass.
        public Menu(UserService userService, AuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
        public void ShowCreatedUsers()
        {
            var users = _userService.GetAllUsers();
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
        public bool CreateUser()
        {
            Console.Clear();
            Console.Write("\nVänligen ange uppgifter.");
            Console.Write("\nPersonnummer : ");
            string userName = Console.ReadLine() ?? "";
            if (_userService.UsernameExists(userName))
            {
                Console.WriteLine("\nFel: Personnumret är redan registrerat!");
                Console.ReadKey();
                return false;
            }
            Console.Write("\nLösenord : ");
            string userPass = Console.ReadLine() ?? "";
            Console.Write("\nEmail : ");
            string userEmail = Console.ReadLine() ?? "";
            Console.Write("\nTelefonnummer : ");
            string userPhoneNumber = Console.ReadLine() ?? "";

            bool succesCreatedUser = _userService.CreateUser(true, userName, userPass, userEmail, userPhoneNumber, UserRole.Customer);
            User? user = _userService.GetUserByUsername(userName);

            return true;
        }
        public void ShowOwnerMenu()
        {
            Console.Clear();
            bool userMenu = true;

            int selectedIndex = 0;

            while (userMenu)
            {
                Console.CursorVisible = false;
                Console.Clear();

                if (selectedIndex == 0)
                {
                    Console.WriteLine("\n[X] Se konton");
                }

                else
                {
                    Console.WriteLine("\n[ ] Se konton");
                }

                if (selectedIndex == 1)
                {
                    Console.WriteLine("\n[X] Transaktionshistorik");
                }

                else
                {
                    Console.WriteLine("\n[ ] Transaktionshistorik");
                }
                if (selectedIndex == 2)
                {
                    Console.WriteLine("\n[X] Logga ut");
                }

                else
                {
                    Console.WriteLine("\n[ ] Logga ut");
                }

                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + 3) % 3;

                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % 3;
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedIndex)
                        {
                            case 0:
                                Console.WriteLine("Konton");
                                ShowCreatedUsers();
                                Console.ReadKey();
                                break;

                            case 1:
                                Console.WriteLine("Transaktionshistorik");
                                break;

                            case 2:
                                _authService.Logout();
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
        public void ShowAdminMenu()
        {
            Console.Clear();
            bool userMenu = true;

            int selectedIndex = 0;

            while (userMenu)
            {
                Console.CursorVisible = false;
                Console.Clear();

                if (selectedIndex == 0)
                {
                    Console.WriteLine("\n[X] Skapa nya användare");
                }

                else
                {
                    Console.WriteLine("\n[ ] Skapa nya användare");
                }

                if (selectedIndex == 1)
                {
                    Console.WriteLine("\n[X] Lås upp användares konto");
                }

                else
                {
                    Console.WriteLine("\n[ ] Lås upp användares konto");
                }


                if (selectedIndex == 2)
                {
                    Console.WriteLine("\n[X] Lån");
                }

                else
                {
                    Console.WriteLine("\n[ ] Lån");
                }
                if (selectedIndex == 3)
                {
                    Console.WriteLine("\n[X] Växelkurser");
                }

                else
                {
                    Console.WriteLine("\n[ ] Växelkurser");
                }

                if (selectedIndex == 4)
                {
                    Console.WriteLine("\n[X] Ränta");
                }

                else
                {
                    Console.WriteLine("\n[ ] Ränta");
                }
                if (selectedIndex == 5)
                {
                    Console.WriteLine("\n[X] Logga ut");
                }

                else
                {
                    Console.WriteLine("\n[ ] Logga ut");
                }
                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + 6) % 6;

                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % 6;
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedIndex)
                        {
                            case 0:
                                CreateUser();
                                break;

                            case 1:
                                //Lås upp användare

                                break;

                            case 2:
                                //Lån
                                break;

                            case 3:
                                // Växelkurser
                                break;

                            case 4:
                                //Ränta

                                break;

                            case 5:
                                _authService.Logout();
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
        public void ShowUserMenu()
        {
            Console.Clear();
            bool userMenu = true;

            int selectedIndex = 0;

            while (userMenu)
            {
                Console.CursorVisible = false;
                Console.Clear();

                if (selectedIndex == 0)
                {
                    Console.WriteLine("\n[X] Se konton");
                }

                else
                {
                    Console.WriteLine("\n[ ] Se konton");
                }

                if (selectedIndex == 1)
                {
                    Console.WriteLine("\n[X] Transaktionshistorik");
                }

                else
                {
                    Console.WriteLine("\n[ ] Transaktionshistorik");
                }
                if (selectedIndex == 2)
                {
                    Console.WriteLine("\n[X] Logga ut");
                }

                else
                {
                    Console.WriteLine("\n[ ] Logga ut");
                }

                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + 3) % 3;

                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % 3;
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedIndex)
                        {
                            case 0:
                                Console.WriteLine("Konton");
                                ShowCreatedUsers();
                                Console.ReadKey();
                                break;

                            case 1:
                                Console.WriteLine("Transaktionshistorik");
                                break;

                            case 2:
                                _authService.Logout();
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
        public void LoginMenu()
        {
            Console.Clear();
            bool userMenu = true;

            int selectedIndex = 0;

            while (userMenu)
            {
                Console.CursorVisible = false;
                Console.Clear();

                if (selectedIndex == 0)
                {
                    Console.WriteLine("\n[X] Logga in");
                }

                else
                {
                    Console.WriteLine("\n[ ] Logga in");
                }

                if (selectedIndex == 1)
                {
                    Console.WriteLine("\n[X] Skapa konto");
                }

                else
                {
                    Console.WriteLine("\n[ ] Skapa konto");
                }

                if (selectedIndex == 2)
                {
                    Console.WriteLine("\n[X] Avsluta");
                }

                else
                {
                    Console.WriteLine("\n[ ] Avsluta");
                }

                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + 3) % 3;

                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % 3;
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedIndex)
                        {
                            case 0:
                                Console.CursorVisible = true;
                                bool wrongUserInfo = true;

                                while (wrongUserInfo)
                                {
                                    Console.Clear();
                                    Console.Write("\nPersonnummer : ");
                                    string userName = Console.ReadLine() ?? "";
                                    Console.Write("\nLösenord : ");
                                    string userPassword = Console.ReadLine() ?? "";
                                    LoginResult userLoggedIn = _authService.Login(userName, userPassword);

                                    if (userLoggedIn == LoginResult.WrongCredentials)
                                    {
                                        Console.CursorVisible = false;
                                        Console.WriteLine("\nFelaktiga uppgifter!" +
                                            "\nTryck valfri knapp för att försöka igen." +
                                            "\nTryck Esc för att avsluta");

                                        key = Console.ReadKey(true).Key;
                                        if (key == ConsoleKey.Escape)
                                        {
                                            wrongUserInfo = false;
                                        }
                                    }

                                    if (userLoggedIn == LoginResult.Success)
                                    {
                                        Console.WriteLine("\nInloggning lyckades!");
                                        if (_authService.IsLoggedInUserAdmin())
                                        {
                                            ShowAdminMenu();
                                        }

                                        else
                                        {
                                            ShowUserMenu();
                                        }
                                        wrongUserInfo = false;
                                    }

                                    if (userLoggedIn == LoginResult.LockedOut)
                                    {
                                        Console.WriteLine("Ditt konto är låst!");
                                        wrongUserInfo = false;
                                    }
                                }

                                Console.ReadKey();
                                break;

                            case 1:
                                Console.CursorVisible = true;
                                CreateUser();
                                Console.ReadKey();
                                break;

                            case 2:
                                Console.CursorVisible = true;
                                Console.WriteLine("Avsluta");
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


