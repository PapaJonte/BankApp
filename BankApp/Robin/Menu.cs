
namespace BankApp.Robin
{
    internal class Menu
    {

        // Creates private fields for UserService and AuthService
        private readonly UserService _userService;
        private readonly AuthService _authService;
        // CONSTRUCTOR INJECTION: We require UserService to be passed in when Menu is created

        // Passes both fields into our constructor so we can use the methods from Maria's class.
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
                Console.WriteLine($"\nYour details" +
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
            Console.Write("\nPlease enter your details.");
            Console.Write("\nPersonal identity number: ");
            string userName = Console.ReadLine() ?? "";
            if (_userService.UsernameExists(userName))
            {
                Console.WriteLine("\nError: This personal identity number is already registered!");
                Console.ReadKey();
                return false;
            }
            Console.Write("\nPassword: ");
            string userPass = Console.ReadLine() ?? "";
            Console.Write("\nEmail: ");
            string userEmail = Console.ReadLine() ?? "";
            Console.Write("\nPhone number: ");
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
                    Console.WriteLine("\n[X] View accounts");
                }

                else
                {
                    Console.WriteLine("\n[ ] View accounts");
                }

                if (selectedIndex == 1)
                {
                    Console.WriteLine("\n[X] Transaction history");
                }

                else
                {
                    Console.WriteLine("\n[ ] Transaction history");
                }
                if (selectedIndex == 2)
                {
                    Console.WriteLine("\n[X] Log out");
                }

                else
                {
                    Console.WriteLine("\n[ ] Log out");
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
                                Console.WriteLine("Accounts");
                                ShowCreatedUsers();
                                Console.ReadKey();
                                break;

                            case 1:
                                Console.WriteLine("Transaction history");
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
                    Console.WriteLine("\n[X] Create new users");
                }

                else
                {
                    Console.WriteLine("\n[ ] Create new users");
                }

                if (selectedIndex == 1)
                {
                    Console.WriteLine("\n[X] Unlock user account");
                }

                else
                {
                    Console.WriteLine("\n[ ] Unlock user account");
                }


                if (selectedIndex == 2)
                {
                    Console.WriteLine("\n[X] Loans");
                }

                else
                {
                    Console.WriteLine("\n[ ] Loans");
                }
                if (selectedIndex == 3)
                {
                    Console.WriteLine("\n[X] Exchange rates");
                }

                else
                {
                    Console.WriteLine("\n[ ] Exchange rates");
                }

                if (selectedIndex == 4)
                {
                    Console.WriteLine("\n[X] Interest rate");
                }

                else
                {
                    Console.WriteLine("\n[ ] Interest rate");
                }
                if (selectedIndex == 5)
                {
                    Console.WriteLine("\n[X] Log out");
                }

                else
                {
                    Console.WriteLine("\n[ ] Log out");
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
                                // Unlock user
                                Console.WriteLine("Type in identity number : ");
                                _authService.UnlockUser();
                                break;

                            case 2:
                                // Loans
                                break;

                            case 3:
                                // Exchange rates
                                break;

                            case 4:
                                // Interest rate

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
                    Console.WriteLine("\n[X] View accounts");
                }

                else
                {
                    Console.WriteLine("\n[ ] View accounts");
                }

                if (selectedIndex == 1)
                {
                    Console.WriteLine("\n[X] Transaction history");
                }

                else
                {
                    Console.WriteLine("\n[ ] Transaction history");
                }
                if (selectedIndex == 2)
                {
                    Console.WriteLine("\n[X] Log out");
                }

                else
                {
                    Console.WriteLine("\n[ ] Log out");
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
                                Console.WriteLine("Accounts");
                                ShowCreatedUsers();
                                Console.ReadKey();
                                break;

                            case 1:
                                Console.WriteLine("Transaction history");
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
                    Console.WriteLine("\n[X] Log in");
                }

                else
                {
                    Console.WriteLine("\n[ ] Log in");
                }

                if (selectedIndex == 1)
                {
                    Console.WriteLine("\n[X] Create account");
                }

                else
                {
                    Console.WriteLine("\n[ ] Create account");
                }

                if (selectedIndex == 2)
                {
                    Console.WriteLine("\n[X] Exit");
                }

                else
                {
                    Console.WriteLine("\n[ ] Exit");
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
                                    Console.Write("\nPersonal identity number: ");
                                    string userName = Console.ReadLine() ?? "";
                                    Console.Write("\nPassword: ");
                                    string userPassword = Console.ReadLine() ?? "";
                                    LoginResult userLoggedIn = _authService.Login(userName, userPassword);

                                    if (userLoggedIn == LoginResult.WrongCredentials)
                                    {
                                        Console.CursorVisible = false;
                                        Console.WriteLine("\nIncorrect credentials!" +
                                            "\nPress any key to try again." +
                                            "\nPress Esc to exit");

                                        key = Console.ReadKey(true).Key;
                                        if (key == ConsoleKey.Escape)
                                        {
                                            wrongUserInfo = false;
                                        }
                                    }

                                    if (userLoggedIn == LoginResult.Success)
                                    {
                                        Console.WriteLine("\nLogin successful!");
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
                                        Console.WriteLine("Your account is locked!");
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
                                Console.WriteLine("Exit");
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
