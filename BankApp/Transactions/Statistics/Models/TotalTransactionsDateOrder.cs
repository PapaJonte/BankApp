using BankApp.Transaction_Layer.Interface;
using BankApp.Transactions.Models;
using BankApp.Transactions.Statistics.Models;

namespace BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;

public record struct TotalTransactionsDateOrder(int UserID, IReadOnlyDictionary<int, List<TransactionDate>> TransactionDictionary) : IRequestStatus
{
    public StatusCode StatusCode { get; init; }

    public readonly IReadOnlyList<TransactionDate> GetTransactionDateOrderDescending(int accountNumber)
    {
        if(TransactionDictionary.TryGetValue(accountNumber, out List<TransactionDate>? UnOrderedTransactionList))
        {
            if(UnOrderedTransactionList != null)
            {
                IReadOnlyList<TransactionDate> orderedList = [.. UnOrderedTransactionList.OrderByDescending(x => x.Date)];
                return orderedList;
            }
        }

        return [];
    }

    public readonly IReadOnlyList<TransactionDate> GetTransactionDateOrderAscending(int accountNumber)
    {
        if(TransactionDictionary.TryGetValue(accountNumber, out List<TransactionDate>? UnOrderedTransactionList))
        {
            if(UnOrderedTransactionList != null)
            {
                IReadOnlyList<TransactionDate> orderedList = [.. UnOrderedTransactionList.OrderBy(x => x.Date)];
                return orderedList;
            }
        }

        return [];
    }
}
