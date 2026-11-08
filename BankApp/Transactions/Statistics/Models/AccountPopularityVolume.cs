using System;
using BankApp.Transaction_Layer.Interface;
using BankApp.Transactions.Models;

namespace BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;

public record struct AccountPopularityVolume(int UserID, IReadOnlyList<TotalVolume> PopularAccountOrdered) : IRequestStatus
{
    public StatusCode StatusCode { get; init; }
}
