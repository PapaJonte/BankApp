namespace BankApp.Pull_The_Sack.View;

using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;
using BankApp.Pull_The_Sack.Interface;

/// <summary>Renders a single <see cref="Reel"/> and its spin animation.</summary>
public class ReelView(Reel reel, GlyphMap glyphs) : IView
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