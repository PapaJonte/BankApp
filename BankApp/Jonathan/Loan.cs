using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Jonathan
{
    internal class Loan
    {
        public int UserId { get; set; }
        public int LoanId { get; set; }
        public decimal LoanAmount { get; private set; }
        public decimal RemainingLoanBalance { get; private set; }
        public decimal InterestRate { get; set; }
        public int Duration { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal TotalRepayment { get; set; }
        public LoanStatus Status { get; private set; } = LoanStatus.Pending;
        
        // Repayment tracking
        public DateTime? ApprovalDate { get; private set; }
        public DateTime? NextPaymentDate { get; private set; }
        public int PaymentsCompleted { get; private set; } = 0;
        public decimal InterestPaid { get; private set; } = 0m;
        public int ConsecutiveMissedPayments { get; private set; } = 0;


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

        internal void StartRepaymentSchedule(DateTime approvalDate)
        {
            // Checks that the loan has been approved
            if (Status != LoanStatus.Approved)
            {
                throw new InvalidOperationException("Only approved loans can start a repayment schedule.");
            }

            // Prevents the repayment schedule from being started more than once
            if (ApprovalDate.HasValue)
            {
                throw new InvalidOperationException("The repayment schedule has already started.");
            }

            // Sets the approval date and the first monthly payment date
            ApprovalDate = approvalDate;
            NextPaymentDate = approvalDate.AddMonths(1);
        }

        internal void RecordMonthlyPayment(decimal interestAmount, decimal amortizationAmount)
        {
            // Checks that the repayment schedule has started
            if (!NextPaymentDate.HasValue)
            {
                throw new InvalidOperationException("The repayment schedule has not started");
            }

            // Checks that the payment amounts are valid
            if (interestAmount < 0 || amortizationAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amortizationAmount), "Payment amounts must be valid.");
            }

            // Prevents more payments than the loan duration
            if (PaymentsCompleted >= Duration)
            {
                throw new InvalidOperationException("All monthly payments have already been completed.");
            }

            // Reduces the remaining loan balance by the amortization amount
            ReduceBalance(amortizationAmount);

            // Adds the paid interest to the total intrest paid
            InterestPaid += interestAmount;

            // Increases the number of completed monthly payments
            PaymentsCompleted++;

            // Resets the missed payment counter after a succesful payment
            ConsecutiveMissedPayments = 0;

            // Moves the next payment date forward by one month
            if (PaymentsCompleted < Duration)
            {
                NextPaymentDate = NextPaymentDate.Value.AddMonths(1);
            }
            else
            {
                NextPaymentDate = null;
            }
        }
    }
}
