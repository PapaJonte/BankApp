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
        private readonly CreditScoreService creditScoreService;
        
        private int nextLoandId = 1;

        public LoanService(InterestRuleService interestRuleService, CreditScoreService creditScoreService)
        {
            this.interestRuleService = interestRuleService ?? throw new ArgumentNullException(nameof(interestRuleService));

            this.creditScoreService = creditScoreService ?? throw new ArgumentNullException(nameof(creditScoreService));

            calculator = new InterestCalculator(interestRuleService);
        }


        //
        // LOAN APPLICATIONS
        //

        public Loan? ApplyForLoan(int userId, decimal requestedAmount, int duration, decimal totalAccountBalance)
        {
            // Rejects loan durations outside the allowed range
            if (duration < 12 || duration > 144)
            {
                return null;
            }

            // Fetch the approved and pending loans
            decimal outstandingLoans = CalculateOutstandingLoans(userId);
            decimal pendingLoans = CalculatePendingLoans(userId);

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

            int creditScore = creditScoreService.GetCreditScore(userId);

            decimal interestRate = calculator.GetInterestRate(duration, creditScore);
            decimal totalInterest = calculator.CalculateTotalInterest(requestedAmount, duration, interestRate);

            // Calculates the total amount the customer will repay, including interest.
            decimal totalRepayment = requestedAmount + totalInterest;

            // Creates a new loan
            Loan newLoan = new Loan(requestedAmount);

            // Customer and loan identification
            newLoan.UserId = userId;
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

        public decimal CalculateOutstandingLoans(int userId)
        {
            // Gets all loans belonging to the customer
            List<Loan> customerLoans = GetCustomersLoan(userId);

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

        public decimal CalculatePendingLoans(int userId)
        {
            // Gets all loans belonging to the customer
            List<Loan> customerLoans = GetCustomersLoan(userId);

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

        public List<Loan> GetCustomersLoan(int userId)
        {
            // Creates a list to store the customer's loans
            List<Loan> customerLoans = new List<Loan>();

            // Goes through all existing loans
            foreach (Loan loan in loans)
            {
                // Checks if the loan belongs to the customer
                if (loan.UserId == userId)
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
                                decimal outstandingLoans = CalculateOutstandingLoans(loan.UserId);
                                decimal pendingLoans = CalculatePendingLoans(loan.UserId);

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

                            // Starts the monthly repayment schedule if the loan was approved
                            if (newStatus == LoanStatus.Approved)
                            {
                                loan.StartRepaymentSchedule(DateTime.Today);
                            }
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

                    // Checks that the repayment schedule has started
                    if (!loan.NextPaymentDate.HasValue)
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

                    // Gets the current month of the loan
                    int currentMonth = loan.PaymentsCompleted + 1;

                    // Calculates the monthly amortization
                    decimal amortization = calculator.CalculateMonthlyAmortization(loan.LoanAmount, loan.Duration);

                    // Make sure the final payment clears the remaining loan balance
                    if (currentMonth == loan.Duration)
                    {
                        amortization = loan.RemainingLoanBalance;
                    }
                    else
                    {
                        amortization = Math.Min(amortization, loan.RemainingLoanBalance);
                    }

                    // Calculates the interest based on the remaining loan balance
                    decimal interest = calculator.CalculateMonthlyInterest(loan.RemainingLoanBalance, loan.InterestRate);

                    // Calculates the total monthly payment
                    decimal monthlyPayment = amortization + interest;

                    // Checks that the payment matches the required total payment
                    if (paymentAmount != monthlyPayment)
                    {
                        return false;
                    }

                    // Records the payment and updates the repayment schedule
                    loan.RecordMonthlyPayment(interest, amortization);

                    return true;
                }
            }

            return false;
        }

        public decimal GetMonthlyPayment(int loanId)
        {
            // Goes through the loans to find the correct loan
            foreach (Loan loan in loans)
            {
                if (loan.LoanId == loanId)
                {
                    // Checks that the loan is approved and has remaining payments
                    if (loan.Status != LoanStatus.Approved || !loan.NextPaymentDate.HasValue)
                    {
                        return 0m;
                    }

                    // Gets the current month of the loan
                    int currentMonth = loan.PaymentsCompleted + 1;

                    // Calculates the monthly payment
                    return calculator.CalculateMonthlyPayment(loan.LoanAmount, loan.Duration, loan.RemainingLoanBalance, loan.InterestRate, currentMonth);
                }
            }

            return 0m;
        }

        public void UpdateInterestRule(int minimumCreditScore, int maximumCreditScore, decimal newAdjustment)
        {
            interestRuleService.UpdateInterestAdjustment(minimumCreditScore, maximumCreditScore, newAdjustment);
        }
    }
}
