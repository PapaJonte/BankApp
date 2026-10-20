using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Jonathan
{
    internal class Loan
    {
        public int CustomerId { get; set; } // References the customer's UserID
        public int LoanId { get; set; }
        public decimal LoanAmount { get; private set; }
        public decimal RemainingLoanBalance { get; private set; }
        public decimal InterestRate { get; set; }
        public int Duration { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal TotalRepayment { get; set; }
        public LoanStatus Status { get; private set; } = LoanStatus.Pending;

        // Creates a loan and sets its original and remaining balance
        public Loan(decimal amount)
        {
            // Checks if the loan amount is greater than zero
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "The loan amount must be greater than zero.");
            }

            // Sets the original loan amount and remaining balance
            LoanAmount = amount;
            RemainingLoanBalance = amount;
        }

        // Subtracts a repayment from the remaining loan balance
        internal void ReduceBalance(decimal paymentAmount)
        {
            // Checks if the payment amount is greater than zero
            if (paymentAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(paymentAmount), "The payment amount must be greater than zero.");
            }

            // Checks if the payment exceeds the remaining loan balance
            if (paymentAmount > RemainingLoanBalance)
            {
                throw new ArgumentOutOfRangeException(nameof(paymentAmount), "The payment cannot exceed the remaining loan balance.");
            }

            // Updates the remaining loan balance
            RemainingLoanBalance -= paymentAmount;
        }

        // Updates the status of the loan
        internal void UpdateStatus(LoanStatus newStatus)
        {
            // Prevents changing the status of an already processed loan
            if (Status != LoanStatus.Pending)
            {
                throw new InvalidOperationException("Only pending loans can change status.");
            }

            // Only allows approving or rejecting a loan
            if (newStatus != LoanStatus.Approved && newStatus != LoanStatus.Rejected)
            {
                throw new ArgumentException("The new loan status must be Approved or Rejected.");
            }

            Status = newStatus;
        }
    }
}
