namespace BankApp.Pull_The_Sack.API;

/// <summary>
/// Small bridge class that holds the loan amount, returned to bank app once the game is complete
/// </summary>
/// <param name=""></param>
/// <returns></returns>
public class BankBridge : IBankInterface
{
    private decimal _loanAmount;

    public BankBridge(decimal startingLoanAmount)
    {
        _loanAmount = startingLoanAmount;
    }

   /// <summary>
   /// returns current loan amount
   /// </summary>
   /// <param name=""></param>
   /// <returns></returns>
    public decimal GetLoanAmount() => _loanAmount;

    /// <summary>
    /// set current loan amount
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public void SetLoanAmount(decimal amount) => _loanAmount = amount;
}