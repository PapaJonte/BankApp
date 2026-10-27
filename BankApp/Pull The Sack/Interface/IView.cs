using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;

namespace BankApp.Pull_The_Sack.Interface;

/// <summary>
/// Base contract for a screen/panel rendered to the console.
/// </summary>
public interface IView
{
    public bool HasBeenServed { get; set; }
    public GameState GameState { get; set; }
    public ViewState ViewState { get; set; }


    /// <summary>Renders the view to the console.</summary>
    void Render(IConsole console, double dt);
    void ResetView();
}