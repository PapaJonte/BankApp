namespace BankApp.Pull_The_Sack.API;


/// <summary>
/// Interface for small bridge object returning the updated loan amount after game.
/// </summary>
public interface IBankInterface
{
    decimal GetLoanAmount();

    void SetLoanAmount(decimal amount);
}