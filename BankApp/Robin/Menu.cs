using BankApp.GustafsClasses;
using BankApp.Pull_The_Sack.Interface;
using BankApp.Transaction_Layer.API;
using BankApp.Transaction_Layer.DataModel;
using BankApp.Transaction_Layer.Enum;
using Spectre.Console;
namespace BankApp.Robin

{
    internal class Menu
    {
        // Creates private fields for UserService and AuthService

        //private readonly Theme theme;

        private readonly BankTransfer _bankTransfer;
        private readonly AccountManager _accountManager;
        private readonly LoanMenu _loanMenu;
        private readonly AdminLoanMenu _adminLoanMenu;
        private readonly UserService _userService;
        private readonly AuthService _authService;
        // CONSTRUCTOR INJECTION: We require UserService to be passed in when Menu is created

        // Passes both fields into our constructor so we can use the methods from Maria's class.
        public Menu(UserService userService, AuthService authService, LoanMenu loanMenu, AdminLoanMenu adminLoanMenu, AccountManager accountManager, BankTransfer bankTransfer)
        {
            _bankTransfer = bankTransfer ?? throw new ArgumentNullException(nameof(bankTransfer));
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
                /*Gustaf, here. The error is my fault.
                 * You can solve this error by inserting a Currency value as a parameter.
                 * 
                 * If you want to set SEK by default you need to add "Currency.SEK" as an input value.
                 * In order to recieve access to the Currency Enum you may have to add "Enum.GetValues<Currency>()" somewhere above in this class.
                 
                 If you want the user to make the choice of Currency then you may insert the method "CheckCurrency()" as an input.*/
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

        private void ChangeMyPassword()
        {
            Console.Clear();
            Console.Write("Current password: ");
            string oldPassword = ReadMasked();
            Console.Write("New password (min 6 characters): ");
            string newPassword = ReadMasked();
            Console.Write("Repeat new password: ");
            string repeat = ReadMasked();

            if (newPassword != repeat)
            {
                Console.WriteLine("The passwords do not match.");
                return;
            }

            Console.WriteLine(_userService.ChangePassword(_authService.LoggedInUser, oldPassword, newPassword)
                ? "Password changed."
                : "Wrong current password or the new one is too short.");
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
                                Console.Write("Type in identity number : ");
                                string userNameUnlock = Console.ReadLine() ?? "";

                                Console.Write("Set a new temporary password (min 6 characters): ");
                                string tempPassword = ReadMasked();

                                bool userUnlocked = _userService.ResetPassword(_authService.LoggedInUser, userNameUnlock, tempPassword);

                                if (userUnlocked)
                                {
                                    Console.WriteLine($"You unlocked user : {userNameUnlock}. They can now log in with the new password.");
                                }
                                else
                                {
                                    Console.WriteLine("Could not unlock (user not found, no permission or password too short).");
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

        public BankAccount GetBankAccounts()
        {
            List<BankAccount> myAccounts = new List<BankAccount>();

            foreach (BankAccount bankAccount in _accountManager.BankAccountList)
            {
                if (bankAccount.GetUserId() == _authService.LoggedInUser.UserID)

                {
                    myAccounts.Add(bankAccount);
                }
            }
            return ;

        }

        private void ShowTransferMenu()
        {
            string[] options = { "Deposit", "Withdraw", "Transfer" };

            int selectedIndex = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== TRANSACTIONS =====");
                Console.WriteLine("Press Esc to go back.\n");

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

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % options.Length;
                        break;

                    case ConsoleKey.Enter:
                        Console.WriteLine($"\n{options[selectedIndex]} is not available yet.");
                        Console.ReadKey(true);


                        switch (selectedIndex)
                        {
                            case 0:
                                Console.WriteLine($"\nWhich account would you like to deposit to?");

                                List<BankAccount> myAccounts = new List<BankAccount>();

                                foreach (BankAccount bankAccount in _accountManager.BankAccountList)
                                {

                                    if (bankAccount.GetUserId() == _authService.LoggedInUser.UserID)

                                    {
                                        myAccounts.Add(bankAccount);
                                    }
                                    _bankTransfer.DepositMoney();
                                }
                                break;

                            case 1:
                                User? customer = _authService.LoggedInUser;

                                if (customer == null)
                                {
                                    return;
                                }

                                var accounts = _accountManager.BankAccountList.Where(account => account.GetUserId() == customer.UserID).ToList();

                                foreach (BankAccount account in accounts)
                                {
                                    Console.WriteLine($"{account.GetBankAccountNumber()}: {account.GetBalance():N2} {account.GetCurrency()}");
                                }

                                Console.Write("Enter the account number: ");
                                if (!int.TryParse(Console.ReadLine(), out int accountNumber))
                                {
                                    Console.WriteLine("Invalid account number.");
                                    break;
                                }

                                BankAccount? source = accounts.Find(account => account.GetBankAccountNumber() == accountNumber);

                                if (source == null)
                                {
                                    Console.WriteLine("Select one of your listed accounts.");
                                    break;
                                }

                                Currency currency = source.GetCurrency();
                                Console.Write($"Enter the withdrawal amount ({currency})");

                                if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0m)
                                {
                                    Console.WriteLine("Enter a positive amount.");
                                    break;
                                }

                                AccountData accountData = new(customer.UserID, accountNumber, currency);
                                StatusCode status = _bankTransfer.WithdrawMoney(accountData, amount);

                                Console.WriteLine(status == StatusCode.Successful ? $"Withdrew {amount:N2} {currency}."
                                    : "The withdrawal failed. Check that the account has enough funds.");

                                break;

                            case 2:
                                foreach (BankAccount bankAccount in _accountManager.BankAccountList)
                                {
                                    Console.WriteLine($"\nAccount number : {bankAccount.GetBankAccountNumber()}" +
                                        $"\nBalance : {bankAccount.GetBalance()} " +
                                        $"\nCurrency : {bankAccount.GetCurrency()}");
                                }

                                // _bankTransfer.TransferMoney();
                                break;

                        }
                        break;

                    case ConsoleKey.Escape:
                        return;
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
                    "Transfer money",
                    "Transaction history",
                    "Loans",
                    "Change password",
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
                        selectedIndex = (selectedIndex - 1 + 7) % 7;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % 7;
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedIndex)
                        {
                            case 0:
                                Console.Clear();
                                Console.WriteLine("===== ACCOUNTS =====");
                                ShowLoggedInUser();
                                //_accountManager.GetAccounts(_authService.LoggedInUser.UserID);

                                foreach (BankAccount bankAccount in _accountManager.BankAccountList)
                                {
                                    if (bankAccount.GetUserId() == _authService.LoggedInUser.UserID)
                                    {
                                        Console.WriteLine($"\nAccount number : {bankAccount.GetBankAccountNumber()}" +
                                            $"\nBalance : {bankAccount.GetBalance()} " +
                                            $"\nCurrency : {bankAccount.GetCurrency()}");
                                    }
                                }
                                Console.ReadKey();
                                break;

                            case 1:
                                /*Gustaf, here. The error is my fault.
                 * You can solve this error by inserting a Currency value as a parameter.
                 * 
                 * If you want to set SEK by default you need to add "Currency.SEK" as an input value.
                 * In order to recieve access to the Currency Enum you may have to add "Enum.GetValues<Currency>()" somewhere above in this class.
                 
                 If you want the user to make the choice of Currency then you may insert the method "CheckCurrency()" as an input.*/
                                Console.CursorVisible = true;
                                Currency currency = AccountManager.CheckCurrency();
                                _accountManager.CreateAccount(_authService.LoggedInUser.UserID, 0m, currency);
                                break;

                            case 2:
                                ShowTransferMenu();
                                break;

                            case 3:
                                Console.WriteLine("Transaction history");
                                break;

                            case 4:
                                Console.WriteLine("Loans");
                                _loanMenu.ShowLoanMenu(_authService.LoggedInUser.UserID);
                                break;

                            case 5:
                                ChangeMyPassword();
                                Console.ReadKey();
                                break;

                            case 6:
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

        //Method to read user input while masking it with asterisks for password entry.
        private static string ReadMasked()
        {
            var input = new System.Text.StringBuilder();

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return input.ToString();
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (input.Length > 0)
                    {
                        input.Length--;
                        Console.Write("\b \b");   
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    input.Append(key.KeyChar);
                    Console.Write('*');
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
                                    string userPassword = ReadMasked();
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
