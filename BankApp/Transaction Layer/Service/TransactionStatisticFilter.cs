using System;
using BankApp.Transaction_Layer.Controller;
using BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Transaction_Layer.Service;

public class TransactionStatisticFilter(StageTransaction stageTransaction)
{
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


    private StatusCode EvaluateStatusCode<T>(IReadOnlyList<T> list)
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
