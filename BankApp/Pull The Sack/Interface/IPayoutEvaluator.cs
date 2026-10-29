namespace BankApp.Pull_The_Sack.Interface;
using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;

/// <summary>
/// Interface for Payout evaluation.
/// </summary>
public interface IPayoutEvaluator
{
    decimal Evaluate(IReadOnlyList<SlotSymbol> symbols);
}