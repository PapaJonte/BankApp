using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Jonathan
{
    internal class Loan
    {
        public int CustomerId { get; set; } // References the customer's UserID
        public int LoanId { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal RemainingLoanBalance { get; set; }
        public decimal InterestRate { get; set; }
        public int Duration { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal TotalRepayment { get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.Pending;
    }
}
