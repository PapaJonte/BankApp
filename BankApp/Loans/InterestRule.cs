using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Jonathan
{
    /// <summary>
    /// An inclusive credit-score range and its adjustable interest surcharge in percentage points
    /// </summary>
    internal class InterestRule
    {
        // Stores the credit score and adjustment to interest
        public int MinimumCreditScore { get; }
        public int MaximumCreditScore { get; }
        public decimal InterestAdjustment { get; set; }

        public InterestRule(int minimumCreditScore, int maximumCreditScore, decimal interestAdjustment)
        {
            MinimumCreditScore = minimumCreditScore;
            MaximumCreditScore = maximumCreditScore;
            InterestAdjustment = interestAdjustment;
        }
    }
}
