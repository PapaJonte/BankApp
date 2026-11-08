using System;
using BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;
using BankApp.Transaction_Layer.Interface;
using BankApp.Transactions.Models;

namespace BankApp.Transactions.Statistics.Models;

public record struct AccountPopularityCount(int UserID, IReadOnlyList<TotalCount> TotalCounts) : IRequestStatus
{
    public StatusCode StatusCode { get; init; }
}
