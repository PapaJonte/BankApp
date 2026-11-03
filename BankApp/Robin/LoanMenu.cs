using BankApp.Jonathan;
using BankApp.Transaction_Layer.API;
using BankApp.GustafsClasses;
using System;
using System.Collections.Generic;
using System.Text;
using BankApp.GustafsClasses;

namespace BankApp.Robin
{
    internal class LoanMenu
    {
        private readonly LoanService loanService;
        private readonly AccountManager accountManager;
        private readonly BankTransfer bankTransfer;

        // Constructor
        public LoanMenu(LoanService loanService, AccountManager accountManager, BankTransfer bankTransfer)
        {
            this.loanService = loanService;
            this.accountManager = accountManager;
            this.bankTransfer = bankTransfer;
        }

        // Handles loan applications
        public Loan? ApplyForLoan(int userId, decimal requestedAmount, int duration)
        {
            // Gets the customers combined total balance
            decimal totalAccountBalance = accountManager.GetTotalBalance(userId);

            // TO DO: Replace with customer's selected regular bank account
            string destinationAccountNumber = "TEST-ACCOUNT-001";

            // Validates and calculates interest etc.
            Loan? loan = loanService.ApplyForLoan(userId, requestedAmount, duration, totalAccountBalance, destinationAccountNumber);

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

            if (statusChanged && newStatus == LoanStatus.Approved)
            {
                string destinationAccountNumber = loan.DestinationAccountNumber;
                decimal payoutAmount = loan.LoanAmount;

                // TO DO: Validate that the destination is a regular account
                // belonging to the customer before enabling payouts

                // TO DO: Remove the test account restriction only after account ownership
                // and account type validation is implemented

                // Never enable payouts while using TEST-ACCOUNT-001

                // TO DO: bankTransfer.DepositMoney(destinationAccountNumber, payoutAmount);

                Console.WriteLine($"Loan #{loan.LoanId} approved " +
                    $"Payout of {payoutAmount} SEK prepared for {destinationAccountNumber}.");
            }

            // Confirms that the loan status has been updated
            Console.WriteLine($"Loan status changed to {newStatus}.");
            return true;
        }

        // Handles a customer's loan repayment
        // TO DO: Connect to the withdrawal system when ready
        public bool RepayLoan(int userId, int loanId, decimal paymentAmount)
        {
            // Find the requested loan and verify that it belongs to the customer
            Loan? loan = loanService.GetCustomersLoan(userId).Find(loanItem => loanItem.LoanId == loanId);

            if (loan == null)
            {
                Console.WriteLine("Loan not found.");
                return false;
            }

            // Only approved loans can be repaid
            if (loan.Status != LoanStatus.Approved)
            {
                Console.WriteLine("Only approved loans can be repaid.");
                return false;
            }

            // Validate the repayment amount
            if (paymentAmount <= 0 || paymentAmount > loan.RemainingLoanBalance)
            {
                Console.WriteLine("Invalid repayment amount.");
                return false;
            }

            // TO DO: Whitdraw the payment from the customer's selected account

            // TO DO: Only update the loan balance after payment is confirmed

            Console.WriteLine($"Repayment of {paymentAmount} SEK, validated for loan #{loanId}. " +
                $"Payment processing is not yet connected.");
            return true;
        }


        // The actual menu displayed to the customer
        public void ShowLoanMenu(int userId)
        {
            string[] options =
            {
                "Apply for a loan",
                "View my loans",
                "Repay a loan",
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
                                // Apply for a new loan
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

                                // Sends the loan application for validation and processing
                                ApplyForLoan(userId, amount, duration);
                                break;

                            case 1:
                                // Display all loans belonging to the customer
                                List<Loan> loans = loanService.GetCustomersLoan(userId);

                                if (loans.Count == 0)
                                {
                                    Console.WriteLine("You have no loans.");
                                }
                                else
                                {
                                    foreach (Loan loan in loans)
                                    {
                                        Console.WriteLine($"\nLoan ID: {loan.LoanId}");
                                        Console.WriteLine($"Status: {loan.Status}");
                                        Console.WriteLine($"Loan amount: {loan.LoanAmount:N2} SEK");
                                        Console.WriteLine($"Remaining balance: {loan.RemainingLoanBalance:N2} SEK");
                                        Console.WriteLine($"Interest rate: {loan.InterestRate}%");
                                        Console.WriteLine($"Duration: {loan.Duration} months");
                                        Console.WriteLine($"Total interest: {loan.TotalInterest:N2} SEK");
                                        Console.WriteLine($"Total repayment: {loan.TotalRepayment:N2} SEK");
                                        Console.WriteLine("------------------------------");
                                    }
                                }
                                break;

                            case 2:
                                // Validate a repayment request
                                Console.Write("Enter the loan ID: ");

                                if (!int.TryParse(Console.ReadLine(), out int loanId))
                                {
                                    Console.WriteLine("Invalid loan ID.");
                                    break;
                                }

                                Console.Write("Enter the repayment amount (SEK): ");

                                if (!decimal.TryParse(Console.ReadLine(), out decimal paymentAmount))
                                {
                                    Console.WriteLine("Invalid repayment amount.");
                                    break;
                                }

                                // Validates the repayment request
                                RepayLoan(userId, loanId, paymentAmount);
                                break;

                            case 3:
                                // Return to the main menu
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