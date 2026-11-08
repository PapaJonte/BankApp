using BankApp.GustafsClasses;
using BankApp.Transaction_Layer.Interface;
using BankApp.Transactions.Models;

namespace BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;

public record struct MostPopularAccounts(IReadOnlyList<TotalCount>? AccountsOrdered) : IRequestStatus
{
    public StatusCode StatusCode { get; init; }
}