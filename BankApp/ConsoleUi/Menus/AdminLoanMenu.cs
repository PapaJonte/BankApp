using BankApp.Jonathan;
using BankApp.GustafsClasses;
using System;
using System.Collections.Generic;
using System.Text;
using BankApp.Transactions.Transfers;
using BankApp.Transactions.Models;
using BankApp.Currencies;
namespace BankApp.ConsoleUi.Menus
{
    /// <summary>
    /// Admin menu for reviewing applications and updating loan pricing rules
    /// </summary>
    internal class AdminLoanMenu
    {
        private readonly LoanService loanService;
        private readonly AccountManager accountManager;
        private readonly BankTransfer bankTransfer;

        public AdminLoanMenu(LoanService loanService, AccountManager accountManager, BankTransfer bankTransfer)
        {
            this.loanService = loanService;
            this.accountManager = accountManager;
            this.bankTransfer = bankTransfer;
        }

        // Interest-rule settings
        private void UpdateCreditScoreAdjustment()
        {
            // Show the existing brackets so the admin can choose which rule to update
            Console.WriteLine("Credit-score brackets: 0-29, 30-49, 50-69, 70-89, 90-100");

            Console.Write("Minimum score: ");
            bool validMinimum = int.TryParse(Console.ReadLine(), out int minimum);

            Console.Write("Maximum score: ");
            bool validMaximum = int.TryParse(Console.ReadLine(), out int maximum);

            // This adjustment is added to the base rate in percentage points
            Console.Write("Interest adjustment in percentage points: ");
            bool validAdjustment = decimal.TryParse(Console.ReadLine(), out decimal adjustment);

            if(!validMinimum || !validMaximum || !validAdjustment || adjustment < 0)
            {
                Console.WriteLine("Enter a valid score bracket and a non-negative adjustment");
                return;
            }

            try
            {
                // New applications use the updated rule, submitted loans keep their quoted rates
                loanService.UpdateInterestRule(minimum, maximum, adjustment);
                Console.WriteLine("Interest rule updated for new loan application");
            }
            catch (InvalidOperationException)
            {
                // Both bounds must be an existing bracket
                Console.WriteLine("Enter one of the existing score brackets exactly.");
            }
        }

        private void UpdateBaseRate()
        {
            // Show the supported brackets so the admin can enter both bounds
            Console.WriteLine("Duration brackets in months: 12-24, 25-48, 49-72, 73-96, 97-120, 121-144");

            Console.Write("Minimum duration: ");
            bool validMinimum = int.TryParse(Console.ReadLine(), out int minimum);

            Console.Write("Maximum duration: ");
            bool validMaximum = int.TryParse(Console.ReadLine(), out int maximum);

            // Enter the annual rate as a percentage, for example 7 for 7%
            Console.Write("New annual base rate (%): ");
            bool validRate = decimal.TryParse(Console.ReadLine(), out decimal rate);

            // Validate the input before passing it to the service
            if (!validMinimum || !validMaximum || !validRate || rate < 0)
            {
                Console.WriteLine("Enter a valid duration bracket and a non-negative rate.");
                return;
            }

            // Both bounds must match the same existing bracket
            if (!loanService.UpdateBaseInterestRate(minimum, maximum, rate))
            {
                Console.WriteLine("Enter one of the existing duration brackets exactly.");
                return;
            }

            Console.WriteLine("Base rate updated for new loan applications.");
        }

        public void ShowInterestRateMenu()
        {
            string[] options =
            {
                "Update base interest rate",
                "Update credit-score interest adjustment",
                "Return to admin menu"
            };

            int selectedOption = 0;

            while (true)
            {
                Console.Clear();
                Console.CursorVisible = false;
                Console.WriteLine("===== LOAN INTEREST RATES =====\n");

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedOption)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(options[i]);
                    }

                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.UpArrow:
                            selectedOption = (selectedOption - 1 + options.Length) % options.Length;
                            break;

                        case ConsoleKey.DownArrow:
                            selectedOption = (selectedOption + 1) % options.Length;
                            break;

                        case ConsoleKey.Escape:
                            return;

                        case ConsoleKey.Enter:
                            Console.Clear();
                            Console.CursorVisible = true;

                            // Reuse the existing rate prompts and validation.
                            switch (selectedOption)
                            {
                                case 0:
                                    UpdateBaseRate();
                                    break;

                                case 1:
                                    UpdateCreditScoreAdjustment();
                                    break;

                                case 2:
                                    return;
                            }

                            Console.WriteLine("\nPress any key to continue...");
                            Console.ReadKey(true);
                            break;
                    }
                }
            }
        }

        // Displays the menu
        public void ShowAdminLoanMenu(int userId)
        {
            string[] options =
            {
                "View pending loans",
                "Approve a loan or retry payout",
                "Reject a loan",
                "Update credit-score interest adjustment",
                "Update base interest rate",
                "Return to admin menu"
            };

            int selectedOption = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== ADMIN LOAN MENU =====\n");

                // Displays the options and highlights the selected one
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedOption)
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

                // Reads the user's input after displaying all options
                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedOption = (selectedOption - 1 + options.Length) % options.Length;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedOption = (selectedOption + 1) % options.Length;
                        break;

                    case ConsoleKey.Escape:
                        return;

                    case ConsoleKey.Enter:
                        Console.Clear();

                        switch (selectedOption)
                        {
                            case 0:
                                // View pending loans
                                List<Loan> pendingLoans = loanService.GetPendingLoans();

                                if (pendingLoans.Count == 0)
                                {
                                    Console.WriteLine("There are no pending loan applications.");
                                }
                                else
                                {
                                    // Display all pending loans
                                    foreach (Loan pendingLoan in pendingLoans)
                                    {
                                        Console.WriteLine($"\nLoan ID: {pendingLoan.LoanId}\n" +
                                            $"Customer ID: {pendingLoan.UserId}\n" +
                                            $"Loan amount: {pendingLoan.LoanAmount:N2} SEK\n" +
                                            $"Interest rate: {pendingLoan.InterestRate}%\n" +
                                            $"Duration: {pendingLoan.Duration} months\n" +
                                            $"Total interest: {pendingLoan.TotalInterest:N2} SEK\n" +
                                            $"Total repayment: {pendingLoan.TotalRepayment:N2} SEK\n" +
                                            $"Destination account: {pendingLoan.DestinationAccountNumber}\n" +
                                            $"Status: {pendingLoan.Status}\n" +
                                            $"------------------------------");
                                    }
                                }
                                break;

                            case 1:
                                // Approve loans
                                Console.WriteLine("Enter the loan ID: ");

                                if (!int.TryParse(Console.ReadLine(), out int loanId))
                                {
                                    Console.WriteLine("Invalid loan ID.");
                                    break;
                                }

                                // Include approved loans os the admin can retry if queuing failed
                                Loan? loan = loanService.GetLoanById(loanId);

                                if (loan == null || loan.Status == LoanStatus.Rejected)
                                {
                                    Console.WriteLine("No pending or approved loan found.");
                                    break;
                                }

                                if (loan.PayoutRequested || loan.PayoutDate.HasValue)
                                {
                                    Console.WriteLine("The payout has already been requested.");
                                    break;
                                }

                                // Use the borrower's actual regular account and its currency
                                if (!int.TryParse(loan.DestinationAccountNumber, out int accountNumber))
                                {
                                    Console.WriteLine("Invalid destination account number.");
                                    break;
                                }

                                BankAccount? destination = accountManager.FindRegularAccount(loan.UserId, accountNumber);

                                if (destination == null)
                                {
                                    Console.WriteLine("The destination account could not be verified.");
                                    break;
                                }

                                // Recheck the borrowing limit before approving a pending application
                                if (loan.Status == LoanStatus.Pending)
                                {
                                    decimal totalBalance;
                                    try
                                    {
                                        totalBalance = accountManager.GetTotalBalance(loan.UserId);
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        Console.WriteLine("An exchange rate is missing. The loan is still pending.");
                                        break;
                                    }

                                    if (!loanService.ChangeLoanStatus(loanId, LoanStatus.Approved, totalBalance))
                                    {
                                        Console.WriteLine("Loan approval failed.");
                                        break;
                                    }
                                }

                                AccountData target = new(loan.UserId, accountNumber, destination.GetCurrency());
                                StatusCode status = bankTransfer.TransferLoan(target, loan.LoanAmount, Currency.SEK, loanId);

                                if (status != StatusCode.Successful)
                                {
                                    Console.WriteLine($"Loan #{loanId} is approved, but the payout could not be queued. You can retry.");
                                    break;
                                }

                                // Record the request only after the transaction queue accepts it
                                loanService.RequestLoanPayout(loanId);
                                Console.WriteLine($"Loan #{loanId} is approved. Payout queued.");

                                break;

                            case 2:
                                // Reject a loan
                                Console.Write("Enter the loan ID to reject: ");

                                if (!int.TryParse(Console.ReadLine(), out int rejectLoanId))
                                {
                                    Console.WriteLine("Invalid loan ID.");
                                    break;
                                }

                                // Find the loan among pending applications
                                Loan? rejectedLoan = loanService.GetPendingLoans().Find(loanItem => loanItem.LoanId == rejectLoanId);

                                if (rejectedLoan == null)
                                {
                                    Console.WriteLine("Pending loan not found.");
                                    break;
                                }

                                // Attempt to reject a loan
                                bool rejected = loanService.ChangeLoanStatus(rejectLoanId, LoanStatus.Rejected, 0m);

                                if (rejected)
                                {
                                    Console.WriteLine($"Loan #{rejectLoanId} rejected.");
                                }
                                else
                                {
                                    Console.WriteLine("Loan rejection failed.");
                                }

                                break;

                            case 3:
                                // Update credit-score interest adjustment
                                UpdateCreditScoreAdjustment();
                                break;

                            case 4:
                                // Update base interest rate
                                UpdateBaseRate();
                                break;

                            case 5:
                                // Return to the admin menu
                                return;
                        }

                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey(true);
                        break;
                }
            }
        }
    }
}
