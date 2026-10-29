namespace BankApp.Pull_The_Sack.Service;

using System.Security.Cryptography.X509Certificates;
using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;
using BankApp.Pull_The_Sack.Interface;

/// <summary>
/// Evaluates landed symbols against a <see cref="PayoutTable"/>.
/// </summary>
public class PayoutEvaluator(PayoutTable table) : IPayoutEvaluator
{

    /// <summary>
    /// Evaluate payout based on symbols the reels rolled
    /// </summary>
    /// <param name="">
    /// IReadOnlyList<SlotSymbol> symbols rolled by reel, used to evaluate payout.
    /// </param>
    /// <returns></returns>
    public decimal Evaluate(IReadOnlyList<SlotSymbol> symbols)
    {
        decimal payoutMultiplier = .5m;

        var bestCount = symbols
                        .GroupBy(x => x) //skapar IGrouping<key = SlotSymbol, element = IEnumerable<SlotSymbol>>
                        .OrderByDescending(s => s.Count())
                        .First();

        if (bestCount.Count() > 1)
        {
            payoutMultiplier = bestCount.Count() / 3m * table.GetPayoutValue(bestCount.Key);
        }

        return payoutMultiplier;
    }
}