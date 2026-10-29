namespace BankApp.Pull_The_Sack.View;

using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;
using BankApp.Pull_The_Sack.Interface;


/// <summary>
/// Display balance after game. Not in use.
/// </summary>
public class BalanceView : IView
{
    private decimal _displayed;

    public BalanceView(decimal startingBalance)
    {
        _displayed = startingBalance;
    }

    public decimal Target { get; set; }
    public bool HasBeenServed { get; set; } = false;
    public GameState GameState { get; set; }
    public ViewState ViewState { get; set; } = ViewState.Idle;

    public void ResetView()
    {
    }

    public void Render(IConsole console, double dt)
    {
    }

}
