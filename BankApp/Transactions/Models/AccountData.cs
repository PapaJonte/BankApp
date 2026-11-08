using System;
using BankApp.Currencies;

namespace BankApp.Transactions.Models;

public readonly record struct AccountData(int UserID, int AccountNumber, Currency Currency);
