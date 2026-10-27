namespace BankApp.Pull_The_Sack.Data_Model;

/// <summary>
/// Abstraction over a source of randomness so reel outcomes can be
/// swapped out (e.g. deterministic RNG in tests, weighted RNG in prod).
/// </summary>
public interface IRng
{
    /// <summary>Returns a non-negative random integer less than <paramref name="exclusiveMax"/>.</summary>
    int Next(int exclusiveMax);

    /// <summary>Returns a random integer in [<paramref name="inclusiveMin"/>, <paramref name="exclusiveMax"/>).</summary>
    int Next(int inclusiveMin, int exclusiveMax);
}