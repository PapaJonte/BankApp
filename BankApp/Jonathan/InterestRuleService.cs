using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Jonathan
{
    internal class InterestRuleService
    {
        private List<InterestRule> interestRules = new List<InterestRule>()
        {
            new InterestRule(0, 29, 5.0m),
            new InterestRule(30, 49, 3.5m),
            new InterestRule(50, 69, 2.0m),
            new InterestRule(70, 89, 1.0m),
            new InterestRule(90, 100, 0.0m)
        };

        // Stores the base interest rates for different loan durations
        private readonly Dictionary<(int Min, int Max), decimal> baseInterestRates = new Dictionary<(int Min, int Max), decimal>()
        {
            { (12, 24), 6.5m },
            { (25, 48), 7.0m },
            { (49, 72), 7.5m },
            { (73, 96), 8.2m },
            { (97, 120), 8.75m },
            { (121, 144), 9.25m }
        };

        public decimal GetInterestAdjustment(int creditScore)
        {
            if (creditScore < 0 || creditScore > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(creditScore), "Credit score must be between 0 and 100.");
            }

            foreach (InterestRule rule in interestRules)
            {
                if (creditScore >= rule.MinimumCreditScore && creditScore <= rule.MaximumCreditScore)
                {
                    return rule.InterestAdjustment;
                }
            }

            throw new InvalidOperationException("No matching interest rule was found.");
        }

        public void UpdateInterestAdjustment(int minimumCreditScore, int maximumCreditScore, decimal newAdjustment)
        {
            if (newAdjustment < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(newAdjustment), "Interest adjustment cannot be negative.");
            }

            foreach (InterestRule rule in interestRules)
            {
                if (rule.MinimumCreditScore == minimumCreditScore && rule.MaximumCreditScore == maximumCreditScore)
                {
                    rule.InterestAdjustment = newAdjustment;
                    return;
                }
            }

            throw new InvalidOperationException("No matching interest rule was found.");
        }

        // Gets the base interest rate based on loan duration
        public decimal GetBaseInterestRate(int duration)
        {
            foreach (var rate in baseInterestRates)
            {
                if (duration >= rate.Key.Min && duration <= rate.Key.Max)
                {
                    return rate.Value;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(duration), "Loan duration must be between 12 and 144 months.");
        }

        // Updates the base interest rate for an existing duration range
        public bool UpdateBaseInterestRate(int minimumDuration, int maximumDuration, decimal newRate)
        {
            if (newRate < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(newRate), "Interest rate cannot be negative.");
            }

            var durationRange = (minimumDuration, maximumDuration);

            // Checks if the duration range exists
            if (!baseInterestRates.ContainsKey(durationRange))
            {
                return false;
            }

            // Updates the interest rate
            baseInterestRates[durationRange] = newRate;
            return true;
        }
    }
}
