namespace BankApp.Pull_The_Sack.Service;

using BankApp.Pull_The_Sack.Interface;

/// <summary>
/// Default <see cref="IViewUpdater"/>. Holds the active views and
/// runs their update+render pass on each tick.
/// </summary>
public class ViewUpdater : IViewUpdater
{
    private readonly IConsole _console;
    private readonly List<IView> _views = new();

    public ViewUpdater(IConsole console)
    {
        _console = console;
    }

    /// <inheritdoc />
    public void Register(IView view) => _views.Add(view);

    public void DeRegister(IView view) => _views.Remove(view);

    /// <inheritdoc />
    public void Tick(double dt)
    {
        foreach (var view in _views)
        {
            view.Render(_console,dt);
        }
    }
}
