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
    }
}
