using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transaction_Layer.Data;

public class ExchangeRateRegistry
{
    private readonly Dictionary<(Currency, Currency), decimal> exchangeRates = [];

    /// <summary>
    /// Sets the exchange rate between two currencies. It is saved one way, get exchange rate handles the inversion when needed.
    /// </summary>
    /// <param name="">
    /// Currency cur1 first currency
    /// Currency cur2 second currency
    /// decimal exchangeRate
    /// </param>
    /// <returns></returns>
    public void SetExchangeRate(Currency cur1, Currency cur2, decimal exchangeRate)
    {
        if (exchangeRate <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(exchangeRate), "Exchange rate must be positive.");
        }

        if (cur1 == cur2)
        {
            throw new ArgumentException("Cannot set exchange rate between same currencies.");
        }

        exchangeRates[(cur1, cur2)] = exchangeRate;
    }

    /// <summary>
    /// Look for transaction between two currencies. Invert if the order is opposite to the saved one.
    /// </summary>
    /// <param name="">
    /// Currency cur1 the first currency type, order matter.
    /// Currency cur2 the second currency, order matter.
    /// out decimal exchangeRate the exchange rate between the two currencies in their order.
    /// </param>
    /// <returns></returns>
    public bool TryGetExchangeRate(Currency cur1, Currency cur2, out decimal exchangeRate)
    {
        if (exchangeRates.TryGetValue((cur1, cur2), out exchangeRate))
        {
            return true;
        }
        else if (exchangeRates.TryGetValue((cur2, cur1), out exchangeRate))
        {
            exchangeRate = 1 / exchangeRate; // invert exchange rate 
            return true;
        }
        else
        {
            exchangeRate = 0;
            return false;
        }
    }
}
