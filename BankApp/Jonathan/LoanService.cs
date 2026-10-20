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

            // Balance should be total balance across accounts, but excluding loans
            decimal maxLoanAmount = balance * 5;
            return maxLoanAmount;
        }

        public decimal CalculateAvailableCredit(decimal eligibleBalance, decimal totalCommittedLoans)
        {
            if (totalCommittedLoans < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalCommittedLoans), "Outstanding loans cannot be negative");
            }

            // Gets the total amount the customer can borrow
            decimal maxLoanAmount = CalculateMaxLoanAmount(eligibleBalance);

            // Subtract the already existing loans from the max loan amount
            decimal availableCredit = Math.Max(0m, maxLoanAmount - totalCommittedLoans);

            return availableCredit;
        }

        public bool ValidateLoanApplication(decimal requestedAmount, decimal eligibleBalance, decimal totalCommittedLoans)
        {
            // Checks that the requested amount isn't negative
            if (requestedAmount <= 0)
            {
                return false;
            }

            decimal availableCredit = CalculateAvailableCredit(eligibleBalance, totalCommittedLoans);

            // Checks if the request loan amount doesn't exceed the available credit
            if (requestedAmount <= availableCredit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private readonly List<Loan> loans = new List<Loan>();
        private int nextLoandId = 1;

        public Loan? ApplyForLoan(int customerId, decimal requestedAmount, int duration, decimal eligibleBalance)
        {
            // Fetch the approved and pending loans
            decimal outstandingLoans = CalculateOutstandingLoans(customerId);
            decimal pendingLoans = CalculatePendingLoans(customerId);

            // Adds the approved and pending loans together
            decimal totalCommittedLoans = outstandingLoans + pendingLoans;
            
            // Checks if the customer is eligible for a loan
            bool validLoan = ValidateLoanApplication(requestedAmount, eligibleBalance, totalCommittedLoans);

            // If not then the application is denied
            if (!validLoan)
            {
                return null;
            }

            // Fetch calculators from our InterestCalculator class
            InterestCalculator calculator = new InterestCalculator();

            decimal interestRate = calculator.GetInterestRate(duration);
            decimal totalInterest = calculator.CalculateTotalInterest(requestedAmount, duration, interestRate);

            // Creates a new loan
            Loan newLoan = new Loan();
            newLoan.CustomerId = customerId;
            newLoan.LoanId = nextLoandId;
            nextLoandId++;
            newLoan.LoanAmount = requestedAmount;
            newLoan.RemainingLoanBalance = requestedAmount;
            newLoan.TotalInterest = totalInterest;
            newLoan.InterestRate = interestRate;
            newLoan.Duration = duration;
            // Not done yet, it still needs: LoanId, Storage, Approval and Disbursment

            loans.Add(newLoan);

            return newLoan;
        }

        public List<Loan> GetCustomersLoan(int customerId)
        {
            // Creates a list to store the customer's loans
            List<Loan> customerLoans = new List<Loan>();

            // Goes through all existing loans
            foreach (Loan loan in loans)
            {
                // Checks if the loan belongs to the customer
                if (loan.CustomerId == customerId)
                {
                    customerLoans.Add(loan);
                }
            }

            // Returns all loans belonging to the customer
            return customerLoans;
        }

        public decimal CalculateOutstandingLoans(int customerId)
        {
            // Gets all loans belonging to the customer
            List<Loan> customerLoans = GetCustomersLoan(customerId);

            // Stores the total remaining loan balance
            decimal outstandingLoans = 0m;

            // Goes through each approved loan and adds its remaining balance
            foreach (Loan loan in customerLoans)
            {
                if (loan.Status == "Approved")
                {
                    outstandingLoans += loan.RemainingLoanBalance;
                }
            }

            // Returns the total outstanding debt
            return outstandingLoans;
        }

        public decimal CalculatePendingLoans(int customerId)
        {
            // Gets all loans belonging to the customer
            List<Loan> customerLoans = GetCustomersLoan(customerId);

            // Stores the total pending loan balance
            decimal pendingLoans = 0m;

            // Goes through each pending loan and adds its requested amount
            foreach (Loan loan in customerLoans)
            {
                if (loan.Status == "Pending")
                {
                    pendingLoans += loan.LoanAmount;
                }
            }

            // Returns the total amount of pending loans
            return pendingLoans;
        }

        public bool ChangeLoanStatus(int loanId, string newStatus, decimal eligibleBalance)
        {
            foreach (Loan loan in loans)
            {
                // Checks if the LoanId matches
                if (loan.LoanId == loanId)
                {
                    // Checks if the loan is still pending
                    if (loan.Status == "Pending")
                    {
                        // Checks if the new status is valid
                        if (newStatus == "Approved" || newStatus == "Rejected")
                        {
                            // Checks the borrowing limit before approving the loan
                            if (newStatus == "Approved")
                            {
                                // Gets the customer's approved and pending loans
                                decimal outstandingLoans = CalculateOutstandingLoans(loan.CustomerId);
                                decimal pendingLoans = CalculatePendingLoans(loan.CustomerId);

                                // Calculates the total loans and maximum allowed loan amount
                                decimal totalCommittedLoans = outstandingLoans + pendingLoans;
                                decimal maxLoanAmount = CalculateMaxLoanAmount(eligibleBalance);

                                // Prevents approval if the customer exceeds the loan limit
                                if (totalCommittedLoans > maxLoanAmount)
                                {
                                    return false;
                                }
                            }

                            // Updates the loan status
                            loan.Status = newStatus;
                            return true;
                        }
                    }
                }
            }

            // Returns false if the status couldn't be changed
            return false;
        }
    }
}
