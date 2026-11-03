using BankApp.Transaction_Layer.Enum;
using BankApp.Transaction_Layer.Interface;

namespace BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;

public record struct TotalTransactionsDateOrder(int UserID, int AccountID, IReadOnlyList<TransactionDataObject> UnOrderedTransactionList) : IRequestStatus
{
    public StatusCode StatusCode { get; init; }

    public readonly IReadOnlyList<TransactionDataObject> GetTransactionDateOrderDescending()
    {
        IReadOnlyList<TransactionDataObject> orderedList = [.. UnOrderedTransactionList.OrderByDescending(x => x.TransactionDate)];
        return orderedList;
    }

    public readonly IReadOnlyList<TransactionDataObject> GetTransactionDateOrderAscending()
    {
        IReadOnlyList<TransactionDataObject> orderedList = [.. UnOrderedTransactionList.OrderBy(x => x.TransactionDate)];
        return orderedList;
    }
}
