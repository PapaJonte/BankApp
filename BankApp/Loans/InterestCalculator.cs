using System;

namespace BankApp.Jonathan
{
    /// <summary>
    /// Calculates loan quotes and installments using equal principal repayments
    /// </summary>
    internal class InterestCalculator
    {
        //
        // CLASS FIELDS
        //

        private readonly InterestRuleService interestRuleService;

        public InterestCalculator(InterestRuleService interestRuleService)
        {
            this.interestRuleService = interestRuleService ?? throw new ArgumentNullException(nameof(interestRuleService));
        }

        //
        // INTEREST RATES
        //

        // Annual interest rates
        public decimal GetInterestRate(int duration)
        {
            return interestRuleService.GetBaseInterestRate(duration);
        }

        public decimal GetInterestRate(int duration, int creditScore)
        {
            // Gets the base interest rate based on the loan duration
            decimal baseInterestRate = GetInterestRate(duration);
            // Gets the additional interest based on customer's credit score
            decimal interestAdjustment = interestRuleService.GetInterestAdjustment(creditScore);
            // Adds the interest adjustment to the base interest rate in percentage points
            return baseInterestRate + interestAdjustment;
        }


        //
        // MONTHLY CALCULATIONS
        //

        // Monthly installment
        public decimal CalculateMonthlyAmortization(decimal amount, int duration)
        {
            // Safeguard for amount and duration of loans
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Loan amount must be greater than zero.");
            }

            if (duration < 12 || duration > 144)
            {
                throw new ArgumentOutOfRangeException(nameof(duration), "Loan duration must be between 12 and 144 months.");
            }

            // Round down so the final payment can clear the remaining principal
            return Math.Round(amount / duration, 2, MidpointRounding.ToZero);
        }
        
        public decimal CalculateMonthlyInterest(decimal remainingBalance, decimal annualInterestRate)
        {
            // Safeguard for negative outcomes
            if (remainingBalance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(remainingBalance), "The loan's remaining balance cannot be negative.");
            }

            if (annualInterestRate < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(annualInterestRate), "The loan's interest rate cannot be negative.");
            }

            // Rounding it to 2 decimals for ören
            return Math.Round((remainingBalance * annualInterestRate) / (100 * 12), 2);
        }

        public decimal CalculateMonthlyPayment(decimal amount, int duration, decimal remainingBalance, decimal annualInterestRate, int currentMonth)
        {
            // Checks that the remaining balance isn't negative
            if (remainingBalance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(remainingBalance), "The remaining balance cannot be negative.");
            }

            // Check that the current month is within the loan duration
            if (currentMonth < 1 || currentMonth > duration)
            {
                throw new ArgumentOutOfRangeException(nameof(currentMonth), "Current month must be within the loan duration.");
            }

            // Calculates the normal monthly amortization
            decimal amortization = CalculateMonthlyAmortization(amount, duration);

            // Makes sure the final payment clears the remaining loan balance, the final installment repays any principal left by rounding
            if (currentMonth == duration)
            {
                amortization = remainingBalance;
            }
            else
            {
                // Prevents amortization from exceeding the remaining balance
                amortization = Math.Min(amortization, remainingBalance);
            }

            // Calculates the interest based on the remaining loan balance
            decimal interest = CalculateMonthlyInterest(remainingBalance, annualInterestRate);

            // Adds the interest on top of the amortization to get the monthly payment
            return amortization + interest;
        }


        //
        // TOTAL LOAN CALCULATIONS
        //

        // Full-term quote
        public decimal CalculateTotalInterest(decimal amount, int duration, decimal annualInterestRate)
        {
            // Starts with the full loan amount and no accumulated interest
            decimal remainingBalance = amount;
            decimal totalInterest = 0m;
            decimal monthlyAmortization = CalculateMonthlyAmortization(amount, duration);

            // Loops through every month of the loan, simulate the schedule and round each month's interest before summing
            for (int month = 1; month <= duration; month++)
            {
                decimal monthlyInterest = CalculateMonthlyInterest(remainingBalance, annualInterestRate);

                // Adds the monthly interest and subtracts the amortization from the remaining balance
                totalInterest += monthlyInterest;

                if (month == duration)
                {
                    remainingBalance = 0m;
                }
                else
                {
                    // Math.Min prevents the remaining balance from becoming negative
                    remainingBalance -= Math.Min(monthlyAmortization, remainingBalance);
                }
            }

            // Returns the total interest paid over the entire loan
            return totalInterest;
        }

        public decimal CalculateTotalRepayment(decimal amount, int duration, decimal annualInterestRate)
        {
            decimal totalInterest = CalculateTotalInterest(amount, duration, annualInterestRate);

            // Adds the original loan amount to the total interest
            return amount + totalInterest;
        }
    }
}
