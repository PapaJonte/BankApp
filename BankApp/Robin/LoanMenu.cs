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
        public void ApplyForLoan(int customerId, decimal requestedAmount, int duration)
        {
            // Gets the customers combined total balance
            decimal totalBalance = accountManager.GetTotalBalance();

            // Validates and calculates interest etc.
            Loan? loan = loanService.ApplyForLoan(customerId, requestedAmount, duration, totalBalance);

            // If the loan application is rejected, LoanService returns null
            if (loan is null)
            {
                Console.WriteLine("Loan application rejected.");
            }

            // Display the loan details
            Console.WriteLine($"Loan ID: {loan.LoanId}\nLoan amount: {loan.LoanAmount} SEK\n" +
                $"Interest rate: {loan.InterestRate}%\nTotal interest: {loan.TotalInterest}\n" +
                $"Total repayment: {loan.TotalRepayment}\nStatus: {loan.Status}");
            }
    }
}
