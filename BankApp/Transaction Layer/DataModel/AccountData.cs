using System;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transaction_Layer.DataModel;

public readonly record struct AccountData(string AccountNumber, Currency Currency);
