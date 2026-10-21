using System;
using BankApp.Transaction_Layer.Data;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transaction_Layer.Service;

namespace BankApp.Transaction_Layer.API;

public class ExchangeRate
{
    private readonly ExchangeRateRegistry exchangeRateRegistry;

    public ExchangeRate()
    {
        exchangeRateRegistry = new();
    }

    /// <summary>
    /// Responsible for setting the exchange rate between two currencies so it can be used in later conversions.
    /// Stores the given rate in the exchange rate registry.
    /// </summary>
    /// <param name="">
    /// Currency cur1 the currency being converted from.
    /// Currency cur2 the currency being converted to.
    /// decimal exchangeRate the rate to apply when converting from cur1 to cur2.
    /// </param>
    /// <returns>
    /// StatusCode representing result of the operation
    /// </returns>
    public StatusCode SetExchangeRate(Currency cur1, Currency cur2, decimal exchangeRate)
    {
        exchangeRateRegistry.SetExchangeRate(cur1, cur2, exchangeRate);
        return StatusCode.Successful;
    }

    /// <summary>
    /// Attempts to retrieve the exchange rate between two currencies.
    /// Looks up the stored rate in the exchange rate registry.
    /// </summary>
    /// <param name="">
    /// Currency cur1 the currency being converted from.
    /// Currency cur2 the currency being converted to.
    /// decimal exchangeRate output value containing the found rate, or zero if no rate was found.
    /// </param>
    /// <returns>
    /// StatusCode representing result of the lookup
    /// </returns>
    public StatusCode TryGetExchangeRate(Currency cur1, Currency cur2, out decimal exchangeRate)
    {
        if (exchangeRateRegistry.TryGetExchangeRate(cur1, cur2, out exchangeRate))
        {
            return StatusCode.Successful;
        }
        else
        {
            exchangeRate = 0;
            return StatusCode.Failed;
        }
    }

    /// <summary>
    /// Attempts to calculate the value of an amount after converting between two currencies.
    /// Retrieves the relevant exchange rate and applies it to the given value.
    /// </summary>
    /// <param name="">
    /// Currency cur1 the currency being converted from.
    /// Currency cur2 the currency being converted to.
    /// decimal value the amount of money to be converted.
    /// decimal calculatedAmount output value containing the converted amount, or zero if conversion failed.
    /// </param>
    /// <returns>
    /// StatusCode representing result of the calculation
    /// </returns>
    public StatusCode TryCalculateExchangeValue(Currency cur1, Currency cur2, decimal value, out decimal calculatedAmount)
    {
        calculatedAmount = 0;
        if (exchangeRateRegistry.TryGetExchangeRate(cur1, cur2, out decimal exchangeRate))
        {
            calculatedAmount = ExchangeRateCalculator.CalculateWithExchangeRate(value, exchangeRate);
            return StatusCode.Successful;
        }
        else
        {
            return StatusCode.Failed;
        }
    }
}

