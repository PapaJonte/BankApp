namespace BankApp.Pull_The_Sack.Interface;


/// <summary>
/// interface for ViewUpdater which updates registered IViews on game loop
/// </summary>
public interface IViewUpdater
{
    void Register(IView view);

    void DeRegister(IView view);

    void Tick(double deltaTime);
}