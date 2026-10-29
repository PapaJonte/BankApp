using System;

namespace BankApp.Pull_The_Sack.API;

public class GamblingFacade
{

    /// <summary>
    /// small api that returns updated loan data after gamble.
    /// </summary>
    /// <param name="loan"></param>
    /// <returns></returns>
    public IBankInterface GambleOnLoan(decimal loan)
    {
        Orchestrator orc = new();
        var controller = orc.Start(loan);
        var bankInterface = controller.PullTheSack();
        return bankInterface;
    }
}
