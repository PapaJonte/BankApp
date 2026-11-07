using BankApp.GustafsClasses;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transaction_Layer.Interface;

namespace BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;

internal record struct MostPopularAccounts(IReadOnlyList<BankAccount> AccountsOrdered) : IRequestStatus
{
    public StatusCode StatusCode { get; init; }
}