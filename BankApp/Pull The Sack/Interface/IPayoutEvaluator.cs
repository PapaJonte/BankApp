namespace BankApp.Pull_The_Sack.Interface;
using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;

public interface IPayoutEvaluator
{    decimal Evaluate(IReadOnlyList<SlotSymbol> symbols);
}