using System;
using System.Collections.Generic;
using System.Text;
using BankApp.Pull_The_Sack.API;

namespace BankApp.Jonathan
{
    /// <summary>
    /// Coordinates loan eligibility, application decisions, and confirmed payments
    /// </summary>
    internal class LoanService
    {
        //
        // CLASS FIELDS
        //

        // Loan records and pricing dependencies
        private readonly List<Loan> loans = new List<Loan>();
        private readonly InterestRuleService interestRuleService;
        private readonly InterestCalculator calculator;
        private readonly CreditScoreService creditScoreService;
        private GamblingFacade gambling;

        private int nextLoandId = 1;

        public LoanService(InterestRuleService interestRuleService, CreditScoreService creditScoreService)
        {
            this.interestRuleService = interestRuleService ?? throw new ArgumentNullException(nameof(interestRuleService));

            this.creditScoreService = creditScoreService ?? throw new ArgumentNullException(nameof(creditScoreService));

            calculator = new InterestCalculator(interestRuleService);
            gambling = new();
        }


        //
        // LOAN APPLICATIONS
        //

        // Applications and validation
        public Loan? ApplyForLoan(int userId, decimal requestedAmount, int duration, decimal totalAccountBalance, string destinationAccountNumber)
        {
            if (string.IsNullOrWhiteSpace(destinationAccountNumber))
            {
                return null;
            }

            // Rejects loan durations outside the allowed range
            if (duration < 12 || duration > 144)
            {
                return null;
            }

            // Each scheduled payment must repay at least one öre of principal
            if (requestedAmount < duration / 100m)
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

            // Apply any outstanding score deductions before pricing the new loan
            UpdateLatePayments(userId, DateTime.Today);

            int creditScore = creditScoreService.GetCreditScore(userId);

            decimal interestRate = calculator.GetInterestRate(duration, creditScore);
            decimal totalInterest = calculator.CalculateTotalInterest(requestedAmount, duration, interestRate);

            // Calculates the total amount the customer will repay, including interest.
            decimal totalRepayment = requestedAmount + totalInterest;

            Loan newLoan;

            //  gamble on the loan
            Console.WriteLine();
            Console.WriteLine("=====================================================");
            Console.WriteLine("  FEELING LUCKY? GAMBLE ON YOUR LOAN TODAY!");
            Console.WriteLine("  Sponsored by LiveForGambling.com");
            Console.WriteLine("  -- Where every debt is just one spin away --");
            Console.WriteLine("=====================================================");
            Console.WriteLine($"Your current total repayment: {totalRepayment:N2} SEK");
            Console.Write("Do you want to gamble on your loan? (y/n): ");

            string? gambleChoice = Console.ReadLine();

    
            if (!string.IsNullOrWhiteSpace(gambleChoice) &&
                gambleChoice.Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
            {

                requestedAmount = gambling.GambleOnLoan(totalRepayment).GetLoanAmount();
                newLoan = new Loan(requestedAmount, duration, interestRate, totalInterest, totalRepayment);
            }
            else
            {
                Console.WriteLine("Got to risk it to get the biscuit! Keeping your loan as it is.");
                newLoan = new Loan(requestedAmount, duration, interestRate, totalInterest);
            }

            // Creates a new loan
            Console.Clear();
            // Customer and loan identification
            newLoan.UserId = userId;
            newLoan.LoanId = nextLoandId;
            nextLoandId++;

            // Sets an account for the loan deposit
            newLoan.SetDestinationAccount(destinationAccountNumber);

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

        // Borrowing limit and existing commitments
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

        // Loan lists and application decisions
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

                            return true;
                        }
                    }
                }
            }

            // Returns false if the status couldn't be changed
            return false;
        }

        // Payout requests and confirmation
        public bool RequestLoanPayout(int loanId)
        {
            Loan? loan = loans.FirstOrDefault(loanItem => loanItem.LoanId == loanId);

            // Check if the loan exists
            if (loan == null)
            {
                return false;
            }

            // Only approved loans can request a payout
            if (loan.Status != LoanStatus.Approved)
            {
                return false;
            }

            // Prevent duplicate payout requests
            if (loan.PayoutRequested)
            {
                return false;
            }

            loan.MarkPayoutRequested();

            return true;
        }

        public bool ConfirmLoanPayout(int loanId, DateTime creditedDate)
        {
            // Only a confirmed deposit for an approved loan starts repayment
            Loan? loan = GetLoanById(loanId);

            if (loan == null || loan.Status != LoanStatus.Approved)
            {
                return false;
            }

            loan.StartRepaymentSchedule(creditedDate);
            return true;
        }

        // Overdue installments
        public void UpdateLatePayments(int userId, DateTime today)
        {
            foreach (Loan loan in GetCustomersLoan(userId))
            {
                // Repeated checks only charge bills that have become due since the last check
                int newDeductions = loan.UpdateLateBills(today);

                if (newDeductions > 0)
                {
                    creditScoreService.DecreaseCreditScore(userId, newDeductions * 10);
                }
            }
        }

        // Repayment recording and installment amounts
        public bool RepayLoan(int loanId, decimal paymentAmount, DateTime submittedDate)
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

                    // A late payment must cover principal, interest and all accumulated fees
                    decimal monthlyPayment = amortization + interest + loan.LateFees;

                    // Checks that the payment matches the required total payment
                    if (paymentAmount != monthlyPayment)
                    {
                        return false;
                    }

                    // Records the payment and updates the repayment schedule
                    loan.RecordMonthlyPayment(interest, amortization, submittedDate);

                    // Award 5 creditscore points for ever 3 consecutive on-time installments
                    int streak = loan.ConsecutiveOnTimePayments;

                    if (streak > 0 && streak % 3 == 0)
                    {
                        creditScoreService.IncreaseCreditScore(loan.UserId, 5);
                    }

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

        // Interest rules for future applications
        public void UpdateInterestRule(int minimumCreditScore, int maximumCreditScore, decimal newAdjustment)
        {
            interestRuleService.UpdateInterestAdjustment(minimumCreditScore, maximumCreditScore, newAdjustment);
        }

        public bool UpdateBaseInterestRate(int minimumDuration, int maximumDuration, decimal newRate)
        {
            // Update the shared rate rules to price new loan applications
            return interestRuleService.UpdateBaseInterestRate(minimumDuration, maximumDuration, newRate);
        }

        // Gets a loan using its unique loan ID
        public Loan? GetLoanById(int loanId)
        {
            // Goes through all loans to find the matching ID
            foreach (Loan loan in loans)
            {
                if (loan.LoanId == loanId)
                {
                    return loan;
                }
            }

            return null;
        }
    }
}
