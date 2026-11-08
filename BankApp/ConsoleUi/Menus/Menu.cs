using BankApp.GustafsClasses;
using BankApp.Pull_The_Sack.Interface;
using BankApp.Transaction_Layer.Controller;
using BankApp.Transaction_Layer.Interface;
using BankApp.Transaction_Layer.Service;
using Spectre.Console;
using System.Transactions;
using System.Net.Security;
using BankApp.Transactions.Transfers;
using BankApp.Transactions.Models;
using BankApp.Transactions.Statistics;
using BankApp.Transactions.Statistics.Models;
using BankApp.Currencies;
using BankApp.ConsoleUi.Display;
namespace BankApp.ConsoleUi.Menus

{
    internal class Menu
    {
        // Creates private fields for UserService and AuthService

        //private readonly Theme theme;

        private readonly EmailService _emailService = new EmailService();
        private readonly BankTransfer _bankTransfer;
        private readonly BankTransferStatistic _bankTransferStatistic;
        private readonly ExchangeRate _exchangeRates;
        private readonly AccountManager _accountManager;
        private readonly LoanMenu _loanMenu;
        private readonly AdminLoanMenu _adminLoanMenu;
        private readonly UserService _userService;
        private readonly AuthService _authService;

        // CONSTRUCTOR INJECTION: We require UserService to be passed in when Menu is created

        // Passes both fields into our constructor so we can use the methods from Maria's class.
        public Menu(UserService userService, AuthService authService, LoanMenu loanMenu, AdminLoanMenu adminLoanMenu, AccountManager accountManager,
            BankTransfer bankTransfer, BankTransferStatistic bankTransferStatistic, ExchangeRate exchangeRates)
        {
            _bankTransfer = bankTransfer ?? throw new ArgumentNullException(nameof(bankTransfer));
            _bankTransferStatistic = bankTransferStatistic ?? throw new ArgumentNullException(nameof(bankTransferStatistic));
            _exchangeRates = exchangeRates ?? throw new ArgumentNullException(nameof(exchangeRates));
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
                    "Update personal details",
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
                        selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;

                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % options.Length;
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
                                UpdateMyDetails();
                                Console.ReadKey();
                                break;

                            case 3:
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

        // User story 16: lets the logged-in user change their own email and phone number.
        private void UpdateMyDetails()
        {
            Console.Clear();
            Console.CursorVisible = true;

            User? user = _authService.LoggedInUser;
            if (user == null)
            {
                Console.WriteLine("No user logged in.");
                return;
            }

            Console.WriteLine("===== UPDATE PERSONAL DETAILS =====");
            Console.WriteLine("Leave a field empty to keep the current value.\n");

            Console.Write($"Email ({user.Email}): ");
            string newEmail = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(newEmail))
                newEmail = user.Email;

            Console.Write($"Phone number ({user.PhoneNumber}): ");
            string newPhone = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(newPhone))
                newPhone = user.PhoneNumber;

            bool updated = _userService.UpdateUserInfo(user, newEmail, newPhone);

            Console.WriteLine(updated
                ? "\nYour details have been updated."
                : "\nInvalid email or phone number. Nothing was changed.");
        }

        private void UpdateExchangeRate()
        {
            Console.Clear();
            Console.CursorVisible = true;

            // Read the direction of the rate, for example EUR to SEK
            Console.Write("From currency (e.g. EUR): ");
            if (!Enum.TryParse(Console.ReadLine(), true, out Currency fromCurrency) || !Enum.IsDefined(fromCurrency))
            {
                Console.WriteLine("Invalid source currency.");
                return;
            }

            Console.Write("To currency (e.g. SEK): ");
            if (!Enum.TryParse(Console.ReadLine(), true, out Currency toCurrency) || !Enum.IsDefined(toCurrency) || fromCurrency == toCurrency)
            {
                Console.WriteLine("Choose a different, valid destination currency.");
                return;
            }

            Console.Write($"How many {toCurrency} equal 1 {fromCurrency}?");
            if (!decimal.TryParse(Console.ReadLine(), out decimal rate) || rate <= 0)
            {
                Console.WriteLine("Enter a positive exchange rate.");
                return;
            }

            // Saving the same pair again replaces its previous rate
            _exchangeRates.SetExchangeRate(fromCurrency, toCurrency, rate);
            Console.WriteLine($"Rate saved. 1 {fromCurrency} = {rate} {toCurrency}");
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
                    "Update personal details",
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
                        selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;

                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % options.Length;
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

                                Console.Write("Set a new password (min 6 characters): ");
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
                                UpdateExchangeRate();
                                Console.ReadKey(true);
                                break;

                            case 4:
                                // Interest rate

                                break;

                            case 5:
                                UpdateMyDetails();
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

        public List<BankAccount> GetBankAccounts()
        {
            List<BankAccount> myAccounts = new List<BankAccount>();

            foreach (BankAccount bankAccount in _accountManager.BankAccountList)
            {
                if (bankAccount.GetUserId() == _authService.LoggedInUser.UserID)

                {
                    bankAccount.GetBankAccountNumber();
                    myAccounts.Add(bankAccount);
                }
            }
            return myAccounts;
        }

        private void TransferBetweenAccounts()
        {
            Console.CursorVisible = true;

            // Only show accounts the signed-in customer can send money from
            List<BankAccount> accounts = GetBankAccounts();

            foreach (BankAccount account in accounts)
            {
                Console.WriteLine($"{account.GetBankAccountNumber()}: {account.GetBalance():N2} {account.GetCurrency()}");
            }

            Console.Write("Source account number: ");

            if (!int.TryParse(Console.ReadLine(), out int sourceNumber))
            {
                Console.WriteLine("Invalid source account number,");
                return;
            }

            BankAccount? source = accounts.Find(account => account.GetBankAccountNumber() == sourceNumber);

            if (source == null)
            {
                Console.WriteLine("Select one of your own accounts.");
                return;
            }

            // Find the recipient by account number without displaying their balance
            Console.Write("Destination account number: ");

            if (!int.TryParse(Console.ReadLine(), out int destinationNumber) || destinationNumber == sourceNumber)
            {
                Console.WriteLine("Enter a different, valid destination account number.");
                return;
            }

            BankAccount? destination = _accountManager.BankAccountList.Find(account => account.GetBankAccountNumber() == destinationNumber);

            if (destination == null)
            {
                Console.WriteLine("Destination account was not found.");
                return;
            }

            Currency sourceCurrency = source.GetCurrency();
            Console.Write($"Amount to send ({sourceCurrency}): ");

            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0m)
            {
                Console.WriteLine("Enter a positive amount.");
                return;
            }

            // The transaction service converst the amount and queues the transfer
            AccountData origin = new(source.GetUserId(), sourceNumber, sourceCurrency);
            AccountData target = new(destination.GetUserId(), destinationNumber, destination.GetCurrency());
            StatusCode status = _bankTransfer.TransferMoney(origin, target, amount, sourceCurrency);

            Console.WriteLine(status == StatusCode.Successful ? "Transfer queued." : "Transfer could not be queued. Check your balance and the exchange rate.");
        }

        private void OpenAccount()
        {
            string[] options = { "Open regular bank account", "Open savings account" };

            int selectedIndex = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== OPEN ACCOUNT =====");
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

                        switch (selectedIndex)
                        {
                            case 0:
                                // Regular bank account
                                OpenRegularAccount();
                                Console.ReadKey(true);
                                break;

                            case 1:
                                // Savings account
                                OpenSavingsAccount();
                                Console.ReadKey(true);
                                break;
                        }
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        private void OpenRegularAccount()
        {
            Console.CursorVisible = true;

            // Open a regular account for the signed-in customer in their chosen currency
            Currency currency = AccountManager.CheckCurrency();
            _accountManager.CreateAccount(_authService.LoggedInUser!.UserID, 0m, currency);

            Console.WriteLine($"Regular bank account opened in {currency}");
        }

        private void OpenSavingsAccount()
        {
            Console.CursorVisible = true;

            // Show the customer's regular account so they can choose where to open savings
            foreach (BankAccount account in GetBankAccounts())
            {
                Console.WriteLine($"{account.GetBankAccountNumber()} ({account.GetCurrency()})");
            }

            Console.Write("Regular account number: ");

            if (!int.TryParse(Console.ReadLine(), out int accountNumber))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            int userId = _authService.LoggedInUser!.UserID;
            Currency currency = AccountManager.CheckCurrency();
            SavingsAccount? savingsAccount = _accountManager.CreateSavingsAccount(userId, accountNumber, currency);

            if (savingsAccount == null)
            {
                Console.WriteLine("Select one of your own regular accounts.");
                return;
            }

            Console.WriteLine($"Savings account {savingsAccount.GetSavingsAccountNumber()} opened in {currency}");
        }

        private void DepositToAccount()
        {
            Console.CursorVisible = true;

            // List only the customer's accounts. A null rate means it is a regular account.
            var destinations = new Dictionary<int, (Currency Currency, decimal? AnnualRate)>();

            foreach (BankAccount account in GetBankAccounts())
            {
                int number = account.GetBankAccountNumber();
                destinations.Add(number, (account.GetCurrency(), null));
                Console.WriteLine($"{number} — Regular ({account.GetCurrency()})");

                foreach (SavingsAccount savings in account.SavingAccounts)
                {
                    int savingsNumber = savings.GetSavingsAccountNumber();
                    destinations.Add(savingsNumber, (savings.GetCurrency(), savings.GetInterest()));
                    Console.WriteLine($"{savingsNumber} — Savings ({savings.GetCurrency()})");
                }
            }

            Console.Write("Destination account number: ");
            if (!int.TryParse(Console.ReadLine(), out int accountNumber) || !destinations.TryGetValue(accountNumber, out var destination))
            {
                Console.WriteLine("Select one of your listed accounts.");
                return;
            }

            Console.Write($"Amount to deposit ({destination.Currency}): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0m)
            {
                Console.WriteLine("Enter a positive amount.");
                return;
            }

            // Queue the deposit in the currency of the selected account.
            AccountData target = new(_authService.LoggedInUser!.UserID, accountNumber, destination.Currency);

            if (_bankTransfer.DepositMoney(target, amount) != StatusCode.Successful)
            {
                Console.WriteLine("The deposit could not be queued.");
                return;
            }

            Console.WriteLine("Deposit queued.");

            // Estimate one year of interest on this deposit at the account's current rate.
            if (destination.AnnualRate is decimal annualRate)
            {
                decimal estimatedInterest = amount * annualRate / 100m;
                Console.WriteLine($"Estimated interest after one year at {annualRate}%: {estimatedInterest:N2} {destination.Currency}.");
            }
        }

        private void ShowTransferMenu()
        {
            string[] options = { "Deposit", "Withdraw", "Transfer", "Open account" };

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


                        switch (selectedIndex)
                        {
                            case 0:
                                // Deposit
                                DepositToAccount();
                                Console.ReadKey(true);
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
                                // Transfer
                                TransferBetweenAccounts();
                                Console.ReadKey(true);
                                break;

                            case 3:
                                // Open account
                                OpenAccount();
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
                    "Update personal details",
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
                        selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % options.Length;
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedIndex)
                        {
                            case 0:
                                Console.Clear();
                                Console.WriteLine("===== ACCOUNTS =====");
                                ShowLoggedInUser();

                                // GetBankAccounts returns only the signed-in customer's regular accounts
                                foreach (BankAccount bankAccount in GetBankAccounts())
                                {
                                    Console.WriteLine($"\nRegular account: {bankAccount.GetBankAccountNumber()}");
                                    Console.WriteLine($"Balance: {bankAccount.GetBalance():N2} {bankAccount.GetCurrency()}");

                                    // Show each savings account under the regular account it belongs to
                                    foreach (SavingsAccount savingsAccount in bankAccount.SavingAccounts)
                                    {
                                        Console.WriteLine($" Savings account: {savingsAccount.GetSavingsAccountNumber()}");
                                        Console.WriteLine($" Balance: {savingsAccount.GetBalance():N2} {savingsAccount.GetCurrency()}");
                                    }
                                }

                                Console.ReadKey();
                                break;

                            case 1:
                                
                                Console.CursorVisible = true;
                                Currency currency = AccountManager.CheckCurrency();
                                _accountManager.CreateAccount(_authService.LoggedInUser.UserID, 0m, currency);
                                break;

                            case 2:
                                ShowTransferMenu();
                                break;

                            case 3:
                                Console.WriteLine("Transaction history");
                                //Provides us with easy access to values of the object.
                                var transactionLog = _bankTransferStatistic.TransactionsByDate(_authService.LoggedInUser.UserID);
                                //Check if log is empty
                                if (transactionLog.TransactionDictionary.Count == 0)
                                {
                                    Console.WriteLine("You have made no transactions.");
                                }
                                else
                                {
                                    /*If dictionary happens to have content then we access the elements by nestled foreach loops.
                                     This loop handles account numbers.*/
                                    foreach (var transactionInstance in transactionLog.TransactionDictionary)
                                    {
                                        Console.WriteLine($"Account number: {transactionInstance.Key } ");
                                        //This loop handles values of transaction ammount.
                                        foreach (TransactionDate transaction in transactionInstance.Value)
                                        {
                                            Console.WriteLine($"Transaction date: {transaction.Date.ToLocalTime()}, ammount: {transaction.Amount:N2}");
                                        }
                                    }
                                }

                                Console.WriteLine("\nPress any key to continue.");
                                Console.ReadKey(true); //Makes sure the user actually gets to see the transaction history log.
                                break;

                            case 4:
                                Console.WriteLine("Loans");
                                _loanMenu.ShowLoanMenu(_authService.LoggedInUser.UserID);

                                break;

                            case 5:
                                UpdateMyDetails();
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
                GoldenSack.WriteLogo();

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

                                    User? lockedCheck = _userService.GetUserByUsername(userName);
                                    bool wasAlreadyLocked = lockedCheck?.IsLockedOut ?? false;

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
                                        Console.WriteLine("Your account is locked! An admin must unlock it.");

                                        if (!wasAlreadyLocked && lockedCheck != null)
                                        {
                                            _emailService.SendEmail(
                                                lockedCheck.Email,
                                                "Your account has been locked",
                                                "Your account was locked after three failed login attempts. An admin needs to review your account and unlock it.");
                                        }

                                        Console.ReadKey();
                                        wrongUserInfo = false;
                                    }

                                    //if (userLoggedIn == LoginResult.LockedOut)
                                    //{
                                    //    Console.WriteLine("Your account is locked!");
                                    //    Console.ReadKey();
                                    //    wrongUserInfo = false;
                                    //}
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
