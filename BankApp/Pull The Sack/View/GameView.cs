namespace BankApp.Pull_The_Sack.View;

using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;
using BankApp.Pull_The_Sack.Interface;
using BankApp.Pull_The_Sack.View.Configs;

/// <summary>Parent layout for the main game screen; owns the child views.</summary>
public class GameView(GameViewConfig config, IReadOnlyList<ReelView> reels, BalanceView balance) : IView
{


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