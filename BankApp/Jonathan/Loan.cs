using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Jonathan
{
    internal class Loan
    {
        // UserId will be taken from the User class in the future
        public int LoanId { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public int Duration { get; set; }
        public decimal TotalInterest { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
