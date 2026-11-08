using System;

namespace BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;

public record struct TotalCount(int AccountNumber, int Count);
