using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Jonathan
{
    internal class Loan
    {
        public int CustomerId { get; set; } // Will be changed when User class is done
        public int LoanId { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal RemainingLoanBalance { get; set; }
        public decimal InterestRate { get; set; }
        public int Duration { get; set; }
        public decimal TotalInterest { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
