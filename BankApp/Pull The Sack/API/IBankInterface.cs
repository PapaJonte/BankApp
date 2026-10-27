namespace BankApp.Pull_The_Sack.API;

/// <summary>
/// Boundary between the slot game and the host bank application.
/// Lets the game read the loan balance and write back the result.
/// </summary>
public interface IBankInterface
{
    /// <summary>Returns the current loan amount the player has locked in.</summary>
    decimal GetLoanAmount();

    /// <summary>Commits the post-game loan amount back to the bank.</summary>
    void SetLoanAmount(decimal amount);
}