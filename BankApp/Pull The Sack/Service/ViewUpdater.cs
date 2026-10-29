namespace BankApp.Pull_The_Sack.Service;

using BankApp.Pull_The_Sack.Interface;

/// <summary>
/// Updates registered IViews with each gameplay loop
/// </summary>
public class ViewUpdater : IViewUpdater
{
    private readonly IConsole _console;
    private readonly List<IView> _views = new();

    public ViewUpdater(IConsole console)
    {
        _console = console;
    }

    /// <summary>
    /// Registers IView to update
    /// </summary>
    /// <param name="view">IView to update each tick</param>
    public void Register(IView view) => _views.Add(view);
    /// <summary>
    /// Removes IView to update
    /// </summary>
    /// <param name="view">Remove IView from tick update</param>
    public void DeRegister(IView view) => _views.Remove(view);

    /// <summary>
    /// Called from gameplay loop to IViews each tick.
    /// </summary>
    /// <param name="dt">Delta Time between ticks. Currently not in use.</param>
    public void Tick(double dt)
    {
        foreach (var view in _views)
        {
            view.Render(_console,dt);
        }
    }
}
