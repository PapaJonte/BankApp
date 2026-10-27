namespace BankApp.Pull_The_Sack.Enum;


/// <summary>Effect a spin has on the player's loan amount.</summary>
public enum SpinOutcome
{
    /// <summary>Loan amount is halved.</summary>
    Halve,

    /// <summary>Loan amount is doubled.</summary>
    Double,

    /// <summary>Loan amount is unchanged.</summary>
    Unchanged
}