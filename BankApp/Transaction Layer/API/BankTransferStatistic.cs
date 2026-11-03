using System;
using BankApp.Transaction_Layer.DataModel.Transaction_Statistical_Objects;
using BankApp.Transaction_Layer.Enum;
using BankApp.Transaction_Layer.Service;

namespace BankApp.Transaction_Layer.API;

public class BankTransferStatistic(TransactionStatisticFilter transactionStatisticFilter)
{
    public AccountPopularityVolume AccountPopularityTotalVolume(int userID)
    {
        StatusCode status = transactionStatisticFilter.TryOrderByTotalVolume(userID, out IReadOnlyList<TotalVolume> orderedAccountList);

        AccountPopularityVolume accPopVol = new(userID, orderedAccountList) { StatusCode = status };

        return accPopVol;
    }

    public AccountPopularityCount AccountPopularityCount(int userID)
    {
        StatusCode status = transactionStatisticFilter.TryOrderByTransactionCount(userID, out IReadOnlyList<TotalCount> orderedAccountList);

        AccountPopularityCount accPopCount = new(userID, orderedAccountList) { StatusCode = status };

        return accPopCount;
    }
    
    

}
