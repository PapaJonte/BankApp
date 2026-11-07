using BankApp.Transaction_Layer.Controller;
using BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transaction_Layer.Service;

public class TransactionStatisticFilter(StageTransaction stageTransaction)
{
    /// <summary>
    /// Handles fetching transaction history and grouping by account number and then 
    /// ordering them by their value. Creates data object and store result in.
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="orderedAccountList"></param>
    /// <returns></returns>
    public StatusCode TryOrderByTotalVolume(int userID, out IReadOnlyList<TotalVolume> orderedAccountList)
    {
        orderedAccountList = [];
        List<TotalVolume> tmpOrdered = [];
        var transactions = stageTransaction.GetTransactionHistoryOnUserID(userID);
        StatusCode status = EvaluateStatusCode(transactions);

        if (status == StatusCode.Successful)
        {
            var orderByAmount = transactions.GroupBy(x => x.AccountNumber)
                        .OrderByDescending(g => g.Sum(x => x.Amount));

            foreach (var grp in orderByAmount)
            {
                tmpOrdered.Add(new TotalVolume(grp.Key, grp.Sum(x => x.Amount)));
            }

            orderedAccountList = tmpOrdered;
        }

        return status;
    }
    /// <summary>
    /// Handles fetching transaction history and grouping by account number and then 
    /// ordering them by how many transactions they have. Creates data object and store result in.
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="orderedAccountList"></param>
    /// <returns></returns>
    public StatusCode TryOrderByTransactionCount(int userID, out IReadOnlyList<TotalCount> orderedAccountList)
    {
        orderedAccountList = [];
        List<TotalCount> tmpOrdered = [];
        var transactions = stageTransaction.GetTransactionHistoryOnUserID(userID);
        StatusCode status = EvaluateStatusCode(transactions);

        if (status == StatusCode.Successful)
        {
            var orderByCount = transactions.GroupBy(x => x.AccountNumber)
                                    .OrderByDescending(g => g.Count());

            foreach (var grp in orderByCount)
            {
                tmpOrdered.Add(new TotalCount(grp.Key, grp.Count()));
            }

            orderedAccountList = tmpOrdered;
        }

        return status;
    }

    /// <summary>
    /// Fetch all transactions per user ID and returns total amount of all transactions in out param.
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="totalAmount"></param>
    /// <returns></returns>
    public StatusCode TryGetFullTransactionAmount(int userID, out decimal totalAmount)
    {
        totalAmount = default;
        var transactions = stageTransaction.GetTransactionHistoryOnUserID(userID);
        StatusCode status = EvaluateStatusCode(transactions);

        if (status == StatusCode.Successful)
        {
            totalAmount = transactions.Sum(x => x.Amount);
        }

        return status;
    }

    /// <summary>
    /// fetches transactions tied to userID and create data object where they are stored and can be parsed.
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="orderedTransactionDict"></param>
    /// <returns></returns>
    public StatusCode TryGetTransactionsByDate(int userID, out IReadOnlyDictionary<int, List<TransactionDate>> orderedTransactionDict)
    {
        orderedTransactionDict = new Dictionary<int, List<TransactionDate>>();
        Dictionary<int, List<TransactionDate>> tmpTransactions = [];

        var transactions = stageTransaction.GetTransactionHistoryOnUserID(userID);
        StatusCode status = EvaluateStatusCode(transactions);

        if (status == StatusCode.Successful)
        {
            var orderedGroup = transactions.GroupBy(x => x.AccountNumber)
                                            .OrderByDescending(o => o.Sum(s => s.Amount));//order the group by accounts with most money since it seems relevant to user.
            foreach (var grp in orderedGroup)
            {
                tmpTransactions.Add(grp.Key, []);
                foreach (var tobj in grp)
                {
                    tmpTransactions[grp.Key].Add(new TransactionDate(tobj.TransactionDate, tobj.Amount));
                }
            }
            orderedTransactionDict = tmpTransactions;
        }

        return status;
    }


    /// <summary>
    /// Evaluates list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    private StatusCode EvaluateStatusCode<T>(List<T> list)
    {
        StatusCode code = StatusCode.Successful;

        if (list == null)
        {
            code = StatusCode.Failed;
        }

        if (code == StatusCode.Successful && list!.Count == 0)
        {
            code = StatusCode.Invalid;
        }

        return code;
    }
}
