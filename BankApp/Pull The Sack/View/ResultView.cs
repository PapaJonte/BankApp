namespace BankApp.Pull_The_Sack.View;

using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;
using BankApp.Pull_The_Sack.Interface;
using BankApp.Pull_The_Sack.View.Configs;

/// <summary>Outcome screen (halve / double / unchanged) with the resulting balance.</summary>
public class ResultView(ResultViewConfig config, SpinResult result) : IView
{
    private int currentFrame = 0;

    public bool HasBeenServed { get; set; } = false;
    public GameState GameState { get; set; }
    public ViewState ViewState { get; set; } = ViewState.Idle;


    public void Render(IConsole console, double dt)
    {
    }

    public void ResetView()
    {
    }



}