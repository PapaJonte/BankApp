using System;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transaction_Layer.DataModel;

public record struct AccountData(string AccountNumber, Currency Currency);
