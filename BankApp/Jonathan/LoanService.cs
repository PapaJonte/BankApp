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

        //
        // CLASS FIELDS
        //

        private readonly List<Loan> loans = new List<Loan>();
        private readonly InterestRuleService interestRuleService;
        private readonly InterestCalculator calculator;
        
        private int nextLoandId = 1;

        public LoanService(InterestRuleService interestRuleService)
        {
            this.interestRuleService = interestRuleService ?? throw new ArgumentNullException(nameof(interestRuleService));

            calculator = new InterestCalculator(interestRuleService);
        }


        //
        // LOAN APPLICATIONS
        //

        public Loan? ApplyForLoan(int customerId, decimal requestedAmount, int duration, decimal totalAccountBalance, int creditScore = 100)
        {
            // Rejects loan durations outside the allowed range
            if (duration < 12 || duration > 144)
            {
                return null;
            }

            // Checks that the customer's credit score is valid
            if (creditScore < 0 || creditScore > 100)
            {
                return null;
            }

            // Fetch the approved and pending loans
            decimal outstandingLoans = CalculateOutstandingLoans(customerId);
            decimal pendingLoans = CalculatePendingLoans(customerId);

            // Adds the approved and pending loans together
            decimal totalCommittedLoans = outstandingLoans + pendingLoans;

            // Calculates how much of the customer's account balance can be used to qualify for a loan.
            decimal eligibleBalance = CalculateEligibleBalance(totalAccountBalance, outstandingLoans);

            // Checks if the customer is eligible for a loan
            bool validLoan = ValidateLoanApplication(requestedAmount, eligibleBalance, totalCommittedLoans);

            // If not then the application is denied
            if (!validLoan)
            {
                return null;
            }

            // Fetch calculators from our InterestCalculator class
            InterestCalculator calculator = new InterestCalculator(interestRuleService);

            decimal interestRate = calculator.GetInterestRate(duration, creditScore);
            decimal totalInterest = calculator.CalculateTotalInterest(requestedAmount, duration, interestRate);

            // Calculates the total amount the customer will repay, including interest.
            decimal totalRepayment = requestedAmount + totalInterest;

            // Creates a new loan
            Loan newLoan = new Loan(requestedAmount);

            // Customer and loan identification
            newLoan.CustomerId = customerId;
            newLoan.LoanId = nextLoandId;
            nextLoandId++;

            // Loan details
            newLoan.Duration = duration;

            // Interest and repayment details
            newLoan.InterestRate = interestRate;
            newLoan.TotalInterest = totalInterest;
            newLoan.TotalRepayment = totalRepayment;

            loans.Add(newLoan);

            return newLoan;
        }

        public bool ValidateLoanApplication(decimal requestedAmount, decimal eligibleBalance, decimal totalCommittedLoans)
        {
            // Checks that the requested amount isn't negative
            if (requestedAmount <= 0)
            {
                return false;
            }

            // Rejects loan amounts with fractions of an öre
            if (requestedAmount != decimal.Round(requestedAmount, 2))
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


        //
        // LOAN CALCULATIONS
        //

        public decimal CalculateEligibleBalance(decimal totalAccountBalance, decimal outstandingLoans)
        {
            if (totalAccountBalance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalAccountBalance), "The balance cannot be negative.");
            }
            if (outstandingLoans < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(outstandingLoans), "The loan balance cannot be negative.");
            }

            // Calculates the customer's eligible balance by subtracting outstanding loans from their total account balance.
            decimal eligibleBalance = Math.Max(0m, totalAccountBalance - outstandingLoans);

            return eligibleBalance;
        }

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
                throw new ArgumentOutOfRangeException(nameof(totalCommittedLoans), "The total committed loan amount cannot be negative.");
            }

            // Gets the total amount the customer can borrow
            decimal maxLoanAmount = CalculateMaxLoanAmount(eligibleBalance);

            // Subtract the already existing loans from the max loan amount
            decimal availableCredit = Math.Max(0m, maxLoanAmount - totalCommittedLoans);

            return availableCredit;
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
                if (loan.Status == LoanStatus.Approved)
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
                if (loan.Status == LoanStatus.Pending)
                {
                    pendingLoans += loan.LoanAmount;
                }
            }

            // Returns the total amount of pending loans
            return pendingLoans;
        }


        //
        // LOAN MANAGEMENT
        //

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

        // Returns all pending loan applications
        public List<Loan> GetPendingLoans()
        {
            List<Loan> pendingLoans = new List<Loan>();

            foreach (Loan loan in loans)
            {
                if (loan.Status == LoanStatus.Pending)
                {
                    pendingLoans.Add(loan);
                }
            }

            return pendingLoans;
        }

        public bool ChangeLoanStatus(int loanId, LoanStatus newStatus, decimal totalAccountBalance)
        {
            foreach (Loan loan in loans)
            {
                // Checks if the LoanId matches
                if (loan.LoanId == loanId)
                {
                    // Checks if the loan is still pending
                    if (loan.Status == LoanStatus.Pending)
                    {
                        // Checks if the new status is valid
                        if (newStatus == LoanStatus.Approved || newStatus == LoanStatus.Rejected)
                        {
                            // Checks the borrowing limit before approving the loan
                            if (newStatus == LoanStatus.Approved)
                            {
                                // Gets the customer's approved and pending loans
                                decimal outstandingLoans = CalculateOutstandingLoans(loan.CustomerId);
                                decimal pendingLoans = CalculatePendingLoans(loan.CustomerId);

                                // Calculates the total loans and maximum allowed loan amount
                                decimal totalCommittedLoans = outstandingLoans + pendingLoans;

                                // Calculates the customer's eligible balance before approving the loan.
                                decimal eligibleBalance = CalculateEligibleBalance(totalAccountBalance, outstandingLoans);
                                decimal maxLoanAmount = CalculateMaxLoanAmount(eligibleBalance);

                                // Prevents approval if the customer exceeds the loan limit
                                if (totalCommittedLoans > maxLoanAmount)
                                {
                                    return false;
                                }
                            }

                            // Updates the loan status
                            loan.UpdateStatus(newStatus);
                            return true;
                        }
                    }
                }
            }

            // Returns false if the status couldn't be changed
            return false;
        }

        public bool RepayLoan(int loanId, decimal paymentAmount)
        {
            // Find the loan
            foreach (Loan loan in loans)
            {
                if (loan.LoanId == loanId)
                {
                    // Checks if the loan has been approved
                    if (loan.Status != LoanStatus.Approved)
                    {
                        return false;
                    }

                    // Checks if the payment amount is greater than zero
                    if (paymentAmount <= 0)
                    {
                        return false;
                    }

                    // Checks if the payment exceeds the remaining loan balance
                    if (paymentAmount > loan.RemainingLoanBalance)
                    {
                        return false;
                    }

                    // Subtracts the payment from the remaining loan balance
                    loan.ReduceBalance(paymentAmount);

                    return true;
                }
            }

            return false;
        }

        public void UpdateInterestRule(int minimumCreditScore, int maximumCreditScore, decimal newAdjustment)
        {
            interestRuleService.UpdateInterestAdjustment(minimumCreditScore, maximumCreditScore, newAdjustment);
        }
    }
}
