using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Jonathan
{
    internal class InterestCalculator
    {
        public decimal GetInterestRate(int duration)
        {
            // Failsafe if months are incorrect for our 1-12 year long loans
            if (duration < 12 || duration > 144)
            {
                throw new ArgumentOutOfRangeException(nameof(duration), "Loan duration must be between 12 and 144 months.");
            }

            // Interest rates for the different durations of the loans, based on the starting duration

            if (duration <= 24) // 1-2 years
                return 6.5m;

            if (duration <= 48) // 3-4 years
                return 7.0m;

            if (duration <= 72) // 5-6 years
                return 7.5m;

            if (duration <= 96) // 7-8 years
                return 8.2m;

            if (duration <= 120) // 9-10 years
                return 8.75m;

            return 9.25m; // 11-12 years

        }

        public decimal CalculateMonthlyAmortization(decimal amount, int duration)
        {
            // Failsafes for amount and duration of loans
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Loan amount must be greater than zero.");
            }

            if (duration < 12 || duration > 144)
            {
                throw new ArgumentOutOfRangeException(nameof(duration), "Loan duration must be between 12 and 144 months.");
            }

            // Rounding it to 2 decimals for ören
            return Math.Round(amount / duration, 2);
        }
        
        public decimal CalculateMonthlyInterest(decimal remainingBalance, decimal annualInterestRate)
        {
            // Failsafes for negative outcomes
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

        public decimal CalculateMonthlyPayment(decimal amount, int duration, decimal remainingBalance, decimal annualInterestRate)
        {
            decimal amortization = CalculateMonthlyAmortization(amount, duration);
            decimal interest = CalculateMonthlyInterest(remainingBalance, annualInterestRate);

            // Adds the interest on top of the amortization to get the monthly payment
            return amortization + interest;
        }

        public decimal CalculateTotalInterest(decimal amount, int duration, decimal annualInterestRate)
        {
            // Starts with the full loan amount and no accumulated interest
            decimal remainingBalance = amount;
            decimal totalInterest = 0m;

            // Loops through every month of the loan
            for (int month = 1; month <= duration; month++)
            {
                decimal monthlyInterest = CalculateMonthlyInterest(remainingBalance, annualInterestRate);
                decimal monthlyAmortization = CalculateMonthlyAmortization(amount, duration);

                // Adds the monthly interest and subtracts the amortization from the remaining balance
                totalInterest += monthlyInterest;
                // Math.Min prevents the remaining balance from becoming negative
                remainingBalance -= Math.Min(monthlyAmortization, remainingBalance);
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
