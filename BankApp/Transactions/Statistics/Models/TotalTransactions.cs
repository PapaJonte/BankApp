using BankApp.Transaction_Layer.Interface;
using BankApp.Transactions.Models;
using BankApp.Currencies;

namespace BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;

public record struct TotalTransactions(IReadOnlyDictionary<Currency, decimal> TotalsByCurrency) : IRequestStatus
{
    public StatusCode StatusCode { get; init; }
}