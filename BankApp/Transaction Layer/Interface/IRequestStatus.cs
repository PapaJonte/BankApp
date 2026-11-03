using System;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transaction_Layer.Interface;

public interface IRequestStatus
{
    public StatusCode StatusCode { get; init;}
}
