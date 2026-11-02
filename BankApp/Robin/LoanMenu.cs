using BankApp.Jonathan;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Robin
{
    internal class LoanMenu
    {
        private readonly LoanService loanService;
        private readonly AccountManager accountManager;

        // Constructor
        public LoanMenu(LoanService loanService, AccountManager accountManager)
        {
            this.loanService = loanService;
            this.accountManager = accountManager;
        }

        // Handles loan applications
        public Loan? ApplyForLoan(int userId, decimal requestedAmount, int duration)
        {
            // Gets the customers combined total balance
            decimal totalAccountBalance = accountManager.GetTotalBalance(userId);

            // Validates and calculates interest etc.
            Loan? loan = loanService.ApplyForLoan(userId, requestedAmount, duration, totalAccountBalance);

            // If the loan application is rejected, LoanService returns null
            if (loan == null)
            {
                Console.WriteLine("Loan application rejected.");
                return null;
            }

            // Display the loan details
            Console.WriteLine($"Loan ID: {loan.LoanId}\nLoan amount: {loan.LoanAmount} SEK\n" +
                $"Interest rate: {loan.InterestRate}%\nTotal interest: {loan.TotalInterest}\n" +
                $"Total repayment: {loan.TotalRepayment}\nStatus: {loan.Status}");
            return loan;
            }

        public bool ChangeLoanStatus(int loanId, LoanStatus newStatus)
        {
            // Gets the loan using its loand ID
            Loan? loan = loanService.GetLoanById(loanId);

            // Checks if the loan exists
            if (loan == null)
            {
                Console.WriteLine("Loan could not be found.");
                return false;
            }

            // Gets the customer's total balance from their bank accounts
            decimal totalAccountBalance = accountManager.GetTotalBalance(loan.UserId);

            // Attempts to update the loan status
            bool statusChanged = loanService.ChangeLoanStatus(loanId, newStatus, totalAccountBalance);
            
            // Checks if the status was succesfully changed
            if (!statusChanged)
            {
                Console.WriteLine("Loan status could not be changed");
                return false;
            }

            // Confirms that the loan status has been updated
            Console.WriteLine($"Loan status changed to {newStatus}.");
            return true;
        }

        // The actual menu displayed
        public void ShowLoanMenu(int userId)
        {
            string[] options =
            {
                "Apply for a loan",
                "View my loans",
                "Return to main menu"
            };

            int selectedOption = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== LOAN MENU =====\n");
                
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
                        Console.WriteLine($"{options[i]}");
                    }

                    // Reads the user's input
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
                                    Console.Write("Enter the loan amount (SEK): ");

                                    if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                                    {
                                        Console.WriteLine("Invalid loan amount.");
                                        break;
                                    }

                                    Console.Write("Enter the duration in months (12-144): ");

                                    if (!int.TryParse(Console.ReadLine(), out int duration))
                                    {
                                        Console.WriteLine("Invalid duration.");
                                        break;
                                    }

                                    ApplyForLoan(userId, amount, duration);
                                    break;

                                case 1:
                                    List<Loan> loans = loanService.GetCustomersLoan(userId);

                                    if (loans.Count == 0)
                                    {
                                        Console.WriteLine("You have no loans.");
                                    }
                                    else
                                    {
                                        foreach (Loan loan in loans)
                                        {
                                            Console.WriteLine($"\nLoan ID: {loan.LoanId}" +
                                                $"\nAmount: {loan.LoanAmount} SEK" +
                                                $"\nRemaining balance: {loan.RemainingLoanBalance} SEK" +
                                                $"\nInterest rate: {loan.InterestRate}%" +
                                                $"\nStatus: {loan.Status}");
                                        }
                                    }
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
    }
}