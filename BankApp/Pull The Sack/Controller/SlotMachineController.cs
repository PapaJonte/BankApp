using BankApp.Pull_The_Sack.API;
using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;
using BankApp.Pull_The_Sack.Interface;
using BankApp.Pull_The_Sack.IO;
using BankApp.Pull_The_Sack.Service;

/// <summary>
/// Orchestrates a single game session: drives phases, mediates between
/// the <see cref="SlotMachine"/> model, the views, and the bank.
/// </summary>
public class SlotMachineController(
    SlotMachine machine,
    IViewUpdater updater,
    IBankInterface bank,
    UserInput userInput,
    ViewRegistry viewRegistry)
{
    private SpinResult? _spinResults;

    public void PullTheSack() => RunSession();

    private void RunSession()
    {
        ClearAllViews();
        machine.StartGame();

        ServeViewToUpdater(ViewType.SplashView);

        while (machine.State.GamePhase != GamePhase.Exiting)
        {
            StateControl(machine.State);
        }
    }

    private void StateControl(GameState gameState)
    {
        IView view;

        switch (gameState.GamePhase)
        {
            case GamePhase.Splash:
                view = viewRegistry.GetView(ViewType.SplashView);
                if (view.ViewState == ViewState.Complete)
                {
                    userInput.ReadKeyCode(ConsoleKey.Spacebar);
                    machine.Spin();
                }
                break;

            case GamePhase.Spinning:
                view = viewRegistry.GetView(ViewType.GameView);
                if (!view.HasBeenServed)
                {
                    RemoveViewFromUpdater(ViewType.SplashView);
                    ServeViewToUpdater(ViewType.GameView);
                    view.GameState = gameState;
                    view.HasBeenServed = true;
                }
                if (userInput.TryReadKeyCode(ConsoleKey.Spacebar))
                {
                    machine.ResolveSpinResult();
                }
                break;
            case GamePhase.Resolving:
                view = viewRegistry.GetView(ViewType.GameView);
                if (view.GameState.GamePhase != gameState.GamePhase)
                {
                    view.GameState = gameState;
                }
                
                if (userInput.TryReadKeyCode(ConsoleKey.Spacebar))
                {
                    if (view.ViewState == ViewState.Complete)
                    {
                        machine.Apply();
                    }
                }
                break;

            case GamePhase.Result:
                view = viewRegistry.GetView(ViewType.ResultView);
                if (!view.HasBeenServed)
                {
                    RemoveViewFromUpdater(ViewType.GameView);
                    ServeViewToUpdater(ViewType.ResultView);
                    view.GameState = gameState;
                    view.HasBeenServed = true;
                }
                if (view.ViewState == ViewState.Complete)
                {
                    userInput.ReadKeyCode(ConsoleKey.Spacebar);
                    bank.SetLoanAmount(423424);
                }
                break;

            case GamePhase.Exiting:
                ServeViewToUpdater(ViewType.ResultView);
                userInput.ReadKeyCode(ConsoleKey.Spacebar);
                machine.EndGame();
                break;
        }
    }

    private void ServeViewToUpdater(ViewType view) => updater.Register(viewRegistry.GetView(view));

    private void RemoveViewFromUpdater(ViewType view)
    {
        IView v = viewRegistry.GetView(view);
        updater.DeRegister(v);
        v.ResetView();
    }

    private void ClearAllViews()
    {
        foreach (IView view in viewRegistry.AllViews)
        {
            updater.DeRegister(view);
        }
    }
}
