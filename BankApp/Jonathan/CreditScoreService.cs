using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Jonathan
{
    internal class CreditScoreService
    {
        // Store each customer's credit score using their customer ID
        private readonly Dictionary<int, int> creditScores = new Dictionary<int, int>();

        public int GetCreditScore(int userId)
        {
            // Checks if the customer already has a credit score
            if (creditScores.TryGetValue(userId, out int creditScore))
            {
                return creditScore;
            }

            creditScores.Add(userId, 100);
            return 100;
        }

        public void UpdateCreditScore(int userId, int newCreditScore)
        {
            // Checks if the new credit score is within the allowed range
            if (newCreditScore < 0 || newCreditScore > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(newCreditScore), "Credit score must be between 0 and 100.");
            }

            // Updates the customer's credit score or creates it if it doesn't exist
            creditScores[userId] = newCreditScore;
        }

        public void DecreaseCreditScore(int userId, int amount)
        {
            // Prevents negative or zero adjustments
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "The decrease amount must be greater than zero.");
            }

            // Gets the customer's current credit score
            int currentCreditScore = GetCreditScore(userId);

            // Decreases the score without going below zero
            int newCreditScore = Math.Max(0, currentCreditScore - amount);

            UpdateCreditScore(userId, newCreditScore);
        }

        public void IncreaseCreditScore(int userId, int amount)
        {
            // Prevents negative or zro adjustments
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "The increase must be greater than zero.");
            }

            // Gets the customer's current credit score
            int currentCreditScore = GetCreditScore(userId);

            // Increases the score without exceeding 100
            int newCreditScore = Math.Min(100, currentCreditScore + amount);

            UpdateCreditScore(userId, newCreditScore);
        }
    }
}
