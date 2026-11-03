using System;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transaction_Layer.Interface;

namespace BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;

public record struct TotalTransactionAmount(int UserID, decimal TotalAmount, IReadOnlyList<TransactionDataObject> FullTransactionList) : IRequestStatus
{
    public StatusCode StatusCode { get; init; }
}