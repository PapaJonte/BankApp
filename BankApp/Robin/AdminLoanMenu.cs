using BankApp.Jonathan;
using BankApp.Transaction_Layer.API;
using BankApp.Transaction_Layer.DataModel;
using BankApp.Transaction_Layer.Enum;
using BankApp.GustafsClasses;
using System;
using System.Collections.Generic;
using System.Text;
namespace BankApp.Robin
{
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

        // Displays the menu
        public void ShowAdminLoanMenu(int userId)
        {
            string[] options =
            {
                "View pending loans",
                "Approve a loan",
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
                                Console.WriteLine("Enter the loan ID to approve:");

                                if (!int.TryParse(Console.ReadLine(), out int loanId))
                                {
                                    Console.WriteLine("Invalid loan ID.");
                                    break;
                                }

                                // Find the loan among pending applications
                                Loan? loan = loanService.GetPendingLoans().Find(loanItem => loanItem.LoanId == loanId);

                                if (loan == null)
                                {
                                    Console.WriteLine("Pending loan not found.");
                                    break;
                                }

                                // Convert the stored destination account number to an integer
                                if (!int.TryParse(loan.DestinationAccountNumber, out int destinationAccountNumber))
                                {
                                    Console.WriteLine("Invalid destination account number.");
                                    break;
                                }

                                // Verify that the destination account belongs to the customer
                                if (!accountManager.AccountOwnershipVerification(loan.UserId, destinationAccountNumber))
                                {
                                    Console.WriteLine("The destination account could not be verified.");
                                    break;
                                }

                                // Retrieve the customer's total account balance
                                decimal totalAccountBalance;

                                try
                                {
                                    totalAccountBalance = accountManager.GetTotalBalance(loan.UserId);
                                }
                                catch (InvalidOperationException)
                                {
                                    Console.WriteLine("Approval could not continue because an exchange rate is unavailable. The loan is still pending");
                                    break;
                                }

                                // Attempt to approve a loan
                                bool approved = loanService.ChangeLoanStatus(loanId, LoanStatus.Approved, totalAccountBalance);

                                if (approved)
                                {
                                    // TO DO: Queue the payout to the selected regular account
                                    // Mark the payout as reuqested only after Alex's method accepts it
                                    // On confirmed deposit, call ConfirmLoanPayout with the actual credit date
                                    Console.WriteLine($"Loan #{loanId} approved. Payout has not been queued.");
                                }
                                else
                                {
                                    Console.WriteLine("Loan approval failed.");
                                }

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
