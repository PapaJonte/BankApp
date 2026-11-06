using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Jonathan
{
    /// <summary>
    /// Keeps the submitted quote, approval decision and repayments stat for one loan
    /// </summary>
    internal class Loan
    {
        // Borrower, loan identity and quoted terms
        public int UserId { get; set; }
        public int LoanId { get; set; }
        public decimal LoanAmount { get; private set; }
        public decimal InterestRate { get; }
        public int Duration { get; }
        public decimal TotalInterest { get; }
        public decimal TotalRepayment { get; }

        // Approval and payout state
        public LoanStatus Status { get; private set; } = LoanStatus.Pending;
        public bool PayoutRequested { get; private set; } = false;

        // Repayment schedule and progress
        public DateTime? PayoutDate { get; private set; }
        public DateTime? NextPaymentDate { get; private set; }
        public decimal RemainingLoanBalance { get; private set; }
        public int PaymentsCompleted { get; private set; } = 0;
        public decimal InterestPaid { get; private set; } = 0m;
        public int LateBillsIssued { get; private set; }
        public int ConsecutiveOnTimePayments { get; private set; }

        // First missed payment adds a 75 SEK fee, the next adds 150 SEK each
        public decimal LateFees => LateBillsIssued == 0 ? 0m : 75m + (LateBillsIssued - 1) * 150m;
        // Adds a 7 day deadline to pay the new penalty bill
        public DateTime? LateBillDeadLine => LateBillsIssued > 0 && NextPaymentDate is DateTime dueDate ? dueDate.AddDays(LateBillsIssued * 7) : null;

        // The regular bank account selected to receive the loan payout
        public string DestinationAccountNumber { get; private set; } = string.Empty;

        // Creates a loan and sets its original and remaining balance
        public Loan(decimal amount, int duration, decimal interestRate, decimal totalInterest)
        {
            // Checks if the loan amount is greater than zero
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "The loan amount must be greater than zero.");
            }

            // Sets the original loan amount and remaining balance
            LoanAmount = amount;
            RemainingLoanBalance = amount;
            Duration = duration;
            InterestRate = interestRate;
            TotalInterest = totalInterest;
            TotalRepayment = amount + totalInterest;
        }

        public Loan(decimal amount, int duration, decimal interestRate, decimal totalInterest, decimal totalRepayment)
        {
            // Checks if the loan amount is greater than zero
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "The loan amount must be greater than zero.");
            }

            // Sets the original loan amount and remaining balance
            LoanAmount = amount;
            RemainingLoanBalance = amount;
            Duration = duration;
            InterestRate = interestRate;
            TotalInterest = totalInterest;
            TotalRepayment = totalRepayment;
        }

        // Assigns the destination account when the loan application is created
        internal void SetDestinationAccount(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                throw new ArgumentException(nameof(accountNumber), "A destination account number is required");
            }

            DestinationAccountNumber = accountNumber;
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

        // Repayment schedule
        internal void StartRepaymentSchedule(DateTime creditedDate)
        {
            // A repeated confirmation must preserve the original repayment schedule
            if (PayoutDate.HasValue)
            {
                return;
            }

            // Checks that the loan has been approved
            if (Status != LoanStatus.Approved)
            {
                throw new InvalidOperationException("Only approved loans can start a repayment schedule.");
            }

            // The first installment is due one month after the payout is credited
            NextPaymentDate = creditedDate.Date.AddMonths(1);
            PayoutDate = creditedDate.Date;
        }

        internal void RecordMonthlyPayment(decimal interestAmount, decimal amortizationAmount, DateTime submittedDate)
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
            if (PaymentsCompleted == Duration - 1)
            {
                amortizationAmount = RemainingLoanBalance;
            }

            ReduceBalance(amortizationAmount);

            // Adds the paid interest to the total intrest paid
            InterestPaid += interestAmount;

            // Increases the number of completed monthly payments
            PaymentsCompleted++;

            // Compare against this installment's due date before advancing the schedule
            ConsecutiveOnTimePayments = submittedDate.Date <= NextPaymentDate.Value.Date ? ConsecutiveOnTimePayments + 1 : 0;

            // The confirmed payment covers the installment and all its late fees
            LateBillsIssued = 0;

            // Moves the next payment date forward by one month
            if (PaymentsCompleted < Duration && RemainingLoanBalance > 0)
            {
                NextPaymentDate = NextPaymentDate.Value.AddMonths(1);
            }
            else
            {
                NextPaymentDate = null;
            }
        }

        // Payout request tracking
        internal void MarkPayoutRequested()
        {
            if (Status != LoanStatus.Approved)
            {
                throw new InvalidOperationException("Only approved loans can have a payout requested.");
            }

            if (PayoutRequested)
            {
                throw new InvalidOperationException("The loan payout has already been requested.");
            }

            PayoutRequested = true;
        }

        // Overdue installments
        internal int UpdateLateBills(DateTime today)
        {
            if (NextPaymentDate is not DateTime dueDate || today.Date <= dueDate.Date)
            {
                return 0;
            }

            // Issue the first bill the day after the due date, then one every 7 days
            int daysLate = (today.Date - dueDate.Date).Days;
            int billsDue = 1 + (daysLate - 1) / 7;

            if (billsDue <= LateBillsIssued)
            {
                return 0;
            }

            // Count only newly issued bills after the first one for score deductions
            int newDeductions = billsDue - Math.Max(1, LateBillsIssued);
            LateBillsIssued = billsDue;
            return newDeductions;
        }
    }
}
