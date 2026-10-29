using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Jonathan
{
    internal class InterestRule
    {
        // Stores the credit score and adjustment to interest
        public int MinimumCreditScore { get; set; }
        public int MaximumCreditScore { get; set; }
        public decimal InterestAdjustment { get; set; }

        public InterestRule(int minimumCreditScore, int maximumCreditScore, decimal interestAdjustment)
        {
            MinimumCreditScore = minimumCreditScore;
            MaximumCreditScore = maximumCreditScore;
            InterestAdjustment = interestAdjustment;
        }
    }
}
