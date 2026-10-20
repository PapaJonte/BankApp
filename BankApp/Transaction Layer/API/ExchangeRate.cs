using System;
using BankApp.Transaction_Layer.Data;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transaction_Layer.Service;

namespace BankApp.Transaction_Layer.API;

public class ExchangeRate(ExchangeRateRegistry exchangeRateRegistry, ExchangeRateCalculator exchangeRateCalculator)
{

    public StatusCode SetExchangeRate(Currency cur1, Currency cur2, decimal exchangeRate)
    {
        return StatusCode.Successful;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
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
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public StatusCode TryCalculateExchangeValue(Currency cur1, Currency cur2, decimal value, out decimal calculatedAmount)
    {
        calculatedAmount = 0;
        if (exchangeRateRegistry.TryGetExchangeRate(cur1, cur2, out decimal exchangeRate))
        {
            calculatedAmount = exchangeRateCalculator.CalculateWithExchangeRate(value, exchangeRate);
            return StatusCode.Successful;
        }
        else
        {
            return StatusCode.Failed;
        }
    }
}
