namespace BankApp.Pull_The_Sack.API;

/// <summary>
/// Integration stub used until the game is wired into the real bank layer.
/// Holds the loan amount in memory.
/// </summary>
public class BankStub : IBankInterface
{
    private decimal _loanAmount;

    public BankStub(decimal startingLoanAmount)
    {
        _loanAmount = startingLoanAmount;
    }

    /// <inheritdoc />
    public decimal GetLoanAmount() => _loanAmount;

    /// <inheritdoc />
    public void SetLoanAmount(decimal amount) => _loanAmount = amount;
}