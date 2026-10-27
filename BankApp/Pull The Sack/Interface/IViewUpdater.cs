namespace BankApp.Pull_The_Sack.Interface;

/// <summary>
/// Drives the per-frame update + render of the currently active views.
/// </summary>
public interface IViewUpdater
{
    /// <summary>Registers a view to be updated/rendered each tick.</summary>
    void Register(IView view);

    void DeRegister(IView view);

    /// <summary>Runs a single update+render pass for the given delta-time.</summary>
    void Tick(double deltaTime);
}