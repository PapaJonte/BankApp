using System;
using BankApp.Transaction_Layer.Interface;
using BankApp.Transactions.Models;

namespace BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;

public record struct TotalTransactionAmount(int UserID, decimal TotalAmount) : IRequestStatus
{
    public StatusCode StatusCode { get; init; }
}