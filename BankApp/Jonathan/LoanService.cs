using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Jonathan
{
    internal class LoanService
    {
        // decimal balance will eventually come from Gustaf
        // It needs to combine all the of the customer's bank accounts
        // The balance need to be converted to SEK if they're in a foreign currency
        public decimal CalculateMaxLoanAmount(decimal balance)
        {
            if (balance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(balance), "The balance cannot be negative.");
            }

            // Need to differentiate loan from other balance
            decimal maxLoanAmount = balance * 5;
            return maxLoanAmount;
        }

        // ValidateLoanApplication()
        // ApplyForLoan()
        // GetCustomersLoan()
    }
}
