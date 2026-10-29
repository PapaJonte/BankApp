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
    }
}