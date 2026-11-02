namespace BankApp.Pull_The_Sack.View;

using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;
using BankApp.Pull_The_Sack.Interface;
using BankApp.Pull_The_Sack.View.Configs;


/// <summary>
/// Displays the user balance after the slot machine game.
/// </summary>
/// <param name="config"></param>
public class ResultView(ResultViewConfig config) : IView
{
    public bool HasBeenServed { get; set; } = false;
    public GameState GameState { get; set; }
    public ViewState ViewState { get; set; } = ViewState.Idle;

    private int viewIndex = 0;
    public void Render(IConsole console, double dt)
    {
        console.Clear();
        console.WriteCentered("");
        console.WriteCentered("");
        console.WriteCentered("");

        console.WriteCentered(config.LogoArt);

        if (GameState.SpinResult!.PayoutMultiplier < 1)
        {
            console.WriteCentered($"Congratulations! You took a {GameState.Balance} loan on a {GameState.Balance * GameState.SpinResult.PayoutMultiplier} Payout! Wow! You truly are an idiot.");
        }
        else
        {
            console.WriteCentered($"Congratulations! You took a {GameState.Balance} loan on a {GameState.Balance * GameState.SpinResult.PayoutMultiplier} Payout!");
        }

        console.WriteCentered("");

        console.WriteCentered("Press 'Space' to go back to your bank.");

        ++viewIndex;

        if (viewIndex > 4)//prevent previous space from also registering on this frame
        {
            ViewState = ViewState.Complete;
        }
    }

    public void ResetView()
    {

    }



}