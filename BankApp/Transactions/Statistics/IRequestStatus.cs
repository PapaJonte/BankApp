using System;
using BankApp.Transactions.Models;

namespace BankApp.Transaction_Layer.Interface;

public interface IRequestStatus
{
    public StatusCode StatusCode { get; init;}
}
