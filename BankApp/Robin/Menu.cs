using Spectre.Console;
using BankApp.Pull_The_Sack.Interface;
using BankApp.GustafsClasses;
using BankApp.Transaction_Layer.Enum;
namespace BankApp.Robin

{
    internal class Menu
    {
        // Creates private fields for UserService and AuthService

        //private readonly Theme theme;

        private readonly AccountManager _accountManager;
        private readonly LoanMenu _loanMenu;
        private readonly AdminLoanMenu _adminLoanMenu;
        private readonly UserService _userService;
        private readonly AuthService _authService;
        // CONSTRUCTOR INJECTION: We require UserService to be passed in when Menu is created

        // Passes both fields into our constructor so we can use the methods from Maria's class.
        public Menu(UserService userService, AuthService authService, LoanMenu loanMenu, AdminLoanMenu adminLoanMenu, AccountManager accountManager)
        {
            _accountManager = accountManager ?? throw new ArgumentNullException(nameof(accountManager));
            _adminLoanMenu = adminLoanMenu ?? throw new ArgumentNullException(nameof(adminLoanMenu));
            _loanMenu = loanMenu ?? throw new ArgumentNullException(nameof(loanMenu));
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
                  $"\n{user.Email}" +
                  $"\n{user.PhoneNumber}" +
                  $"\n{user.Role}");
            }
        }
        public void ShowLoggedInUser()
        {
            User? userDetails = _authService.LoggedInUser;

            if (userDetails == null)
            {
                Console.WriteLine("No users logged in!");
                return;
            }
            Console.WriteLine($"\n{userDetails.Username}" +
                $"\n{userDetails.Email}");

        }
        public bool CreateUser()
        {
            Console.Clear();
            Console.Write("\nPlease enter details.");
            Console.Write("\nPersonal identity number: ");
            string userName = Console.ReadLine() ?? "";
            Console.Write("\nPassword: ");
            string userPass = Console.ReadLine() ?? "";
            Console.Write("\nEmail: ");
            string userEmail = Console.ReadLine() ?? "";
            Console.Write("\nPhone number: ");
            string userPhoneNumber = Console.ReadLine() ?? "";

            /*Maria: Updated to use the new CreateUser, it passes the currently
            logged-in user (instead of plain bools) so the role check happens for real,
            and reads the CreateUserResult enum to show the exact reason on failure instead of a generic true/false.
            */
            CreateUserResult result = _userService.CreateUser(_authService.LoggedInUser, userName, userPass, userEmail, userPhoneNumber, out User? createdUser, UserRole.Customer);

            string userMessage = UserService.GetErrorMessage(result);

            if (result == CreateUserResult.Success)
            {
                _accountManager.CreateAccount(createdUser.UserID, 0m);
                Console.WriteLine(userMessage);
                return true;
            }
            else
            {
                Console.WriteLine(userMessage);
                return false;
            }
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

                string[] options =
                {
                    "View accounts",
                    "Transaction history",
                    "Log out",
                };

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(options[i]);
                    }
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
                                // Create admin
                                Console.Clear();
                                Console.WriteLine("Accounts");
                                ShowCreatedUsers();
                                Console.ReadKey();
                                break;

                            case 1:
                                //
                                Console.Clear();
                                Console.WriteLine("Transaction history");
                                Console.ReadKey();
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
        public void ShowAdminMenu(int userId)
        {
            Console.Clear();
            bool userMenu = true;

            int selectedIndex = 0;

            while (userMenu)
            {
                Console.CursorVisible = false;
                Console.Clear();

                string[] options =
                {
                    "Create new users",
                    "Unlock user account",
                    "Loans",
                    "Exchange rate",
                    "Interest rate",
                    "Log out",
                };

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(options[i]);
                    }
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
                                Console.Clear();

                                bool succesCreatedUser = CreateUser();
                                Console.ReadKey();

                                break;

                            case 1:
                                // Unlock user
                                Console.Clear();
                                Console.WriteLine("Type in identity number : ");
                                string userNameUnlock = Console.ReadLine() ?? "";

                                bool userUnlocked = _authService.UnlockUser(userNameUnlock);

                                if (userUnlocked)
                                {
                                    Console.Write($"You unlocked user : {userNameUnlock}");
                                }
                                else
                                {
                                    Console.WriteLine("User not found!");
                                }
                                Console.ReadKey();
                                break;

                            case 2:
                                // Loans
                                _adminLoanMenu.ShowAdminLoanMenu(_authService.LoggedInUser.UserID);

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
                        _authService.Logout();
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

                string[] options =
                {
                    "View accounts",
                    "Create bank account",
                    "Transaction history",
                    "Loans",
                    "Log out",
                };

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(options[i]);
                    }
                }

                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + 5) % 5;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % 5;
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedIndex)
                        {
                            case 0:
                                Console.WriteLine("Accounts");
                                ShowLoggedInUser();
                                Console.ReadKey();
                                break;

                            case 1:

                                _accountManager.CreateAccount(_userService.GetLoggedInUser().UserId, 0m);

                                break;

                            case 2:
                                Console.WriteLine("Transaction history");
                                break;

                            case 3:
                                Console.WriteLine("Loans");
                                _loanMenu.ShowLoanMenu(_authService.LoggedInUser.UserID);

                                break;

                            case 4:
                                _authService.Logout();
                                userMenu = false;
                                break;
                        }
                        break;

                    case ConsoleKey.Escape:
                        _authService.Logout();
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
                global::BankApp.GoldenSack.WriteLogo();

                string[] options =
                {
                    "Log in",
                    "Exit",
                };

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(options[i]);
                    }
                }

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
                                            ShowAdminMenu(_authService.LoggedInUser.UserID);
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
                                        Console.ReadKey();
                                        wrongUserInfo = false;
                                    }
                                }

                                break;
                            //case 1:
                            //    Console.CursorVisible = true;
                            //    CreateUser();
                            //    Console.ReadKey();
                            //    break;

                            case 1:
                                Console.CursorVisible = true;
                                Console.WriteLine("Exiting....");
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
