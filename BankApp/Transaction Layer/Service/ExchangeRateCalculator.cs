using System;

namespace BankApp.Transaction_Layer.Service;

public class ExchangeRateCalculator
{
    /// <summary>
    /// Calculates exchangerate based on value passed in
    /// </summary>
    /// <param name="">
    /// float val value we want to calculate with exchange rate
    /// float exchangeRate current exchangeRate
    /// </param>
    /// <returns></returns>
    public decimal CalculateWithExchangeRate(decimal val, decimal exchangeRate) => val * exchangeRate; 
}
