using BankApp.Jonathan;
using BankApp.Transaction_Layer.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Robin
{
    internal class AdminLoanMenu
    {
        private readonly LoanService loanService;
        private readonly AccountManager accountManager;

        public AdminLoanMenu(LoanService loanService, AccountManager accountManager)
        {
            this.loanService = loanService;
            this.accountManager = accountManager;
        }

        // Displays the menu
        public void ShowAdminLoanMenu(int userId)
        {
            string[] options =
            {
                "View pending loans",
                "Approva a loan",
                "Reject a loan",
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

                                // Retrieve the customer's total account balance
                                decimal totalAccountBalance = accountManager.GetTotalBalance(loan.UserId);

                                // Attempt to approve a loan
                                bool approved = loanService.ChangeLoanStatus(loanId, LoanStatus.Approved, totalAccountBalance);

                                if (approved)
                                {
                                    Console.WriteLine($"Loan #{loanId} approved.");
                                }
                                // TO DO: Validate the customer's destination account

                                // TO DO: Connect the payout to the transaction system
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
