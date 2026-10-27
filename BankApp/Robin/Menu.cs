
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
        //private readonly User _user;
        // CONSTRUCTOR INJECTION: Vi kräver att UserService skickas in när Menu skapas

        // Skickar in båda fälten i vår constructor för att kunna använda metoderna från marias klass.
        public Menu(UserService userService, AuthService authService)
        {
            //_user = user ?? throw new ArgumentNullException(nameof(user));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
        //public List<User> users = new List<User> { new User(1, "0204", "123", "Robin@outlook.com", "076918", UserRole.Customer) };
        public void ShowCreatedUser()
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
        public void ShowAdminMenu()
        {
            Console.Clear();
            bool userMenu = true;

            int selectedIndex = 0;

            while (userMenu)
            {
                Console.WriteLine("\n[ ] Skapa nya användare" +
                    "\n[ ] Lån" +
                    "\n[ ] Växelkurser" +
                    "\n[ ] Gå tillbaka till startsidan");
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
        public void ShowUserMenu()
        {

            Console.Clear();
            bool userMenu = true;

            int selectedIndex = 0;

            while (userMenu)
            {
                Console.WriteLine("\nSe Konton" +
                    "\nGå tillbaka till startsida");

                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + 2) % 2;

                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % 2;
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
                    Console.WriteLine("[X] Logga in");
                }

                else
                {
                    Console.WriteLine("[ ] Logga in");
                }

                if (selectedIndex == 1)
                {
                    Console.WriteLine("[X] Skapa konto");
                }

                else
                {
                    Console.WriteLine("[ ] Skapa konto");
                }

                if (selectedIndex == 2)
                {
                    Console.WriteLine("[X] Avsluta");
                }

                else
                {
                    Console.WriteLine("[ ] Avsluta");
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
                                Console.Clear();
                                Console.Write("\nPersonnummer : ");
                                string userName = Console.ReadLine() ?? "";
                                Console.Write("\nLösenord : ");
                                string userPassword = Console.ReadLine() ?? "";
                                LoginResult userLoggedIn = _authService.Login(userName, userPassword);

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
                                }
                                else if (userLoggedIn == LoginResult.LockedOut)
                                {

                                    Console.WriteLine("Ditt konto är låst!");
                                }

                                else
                                {
                                    Console.WriteLine("Fel försök igen!");
                                }

                                Console.ReadKey();
                                break;

                            case 1:
                                Console.CursorVisible = true;
                                CreateUser();
                                ShowCreatedUser();
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


