namespace BankApp.Pull_The_Sack.Service;

using BankApp.Pull_The_Sack.Data_Model;

/// <summary>
/// Default <see cref="IRng"/> implementation backed by <see cref="System.Random"/>.
/// </summary>
public class Rng : IRng
{
    private readonly Random _random;

    public Rng(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    /// <inheritdoc />
    public int Next(int exclusiveMax) => _random.Next(exclusiveMax);

    /// <inheritdoc />
    public int Next(int inclusiveMin, int exclusiveMax) => _random.Next(inclusiveMin, exclusiveMax);
}