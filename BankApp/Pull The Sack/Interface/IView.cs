using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;

namespace BankApp.Pull_The_Sack.Interface;

/// <summary>
/// IView interface, used by the different Views for slot game.
/// </summary>
public interface IView
{
    public bool HasBeenServed { get; set; }
    public GameState GameState { get; set; }
    public ViewState ViewState { get; set; }


    void Render(IConsole console, double dt);
    void ResetView();
}