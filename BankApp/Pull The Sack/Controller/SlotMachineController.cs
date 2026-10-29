using BankApp.Pull_The_Sack.API;
using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;
using BankApp.Pull_The_Sack.Interface;
using BankApp.Pull_The_Sack.IO;
using BankApp.Pull_The_Sack.Service;


/// <summary>
/// state machine based router, loops and routes gamelogic while gamestate is not set to exit.
/// calls slotmachine to process the gameplay and update gamephase. 
/// Handles display call for views based on gamephase.
/// </summary>
/// <param name="">
/// SlotMachine machine, handles gameplay logic and owns the link to gameloop.
/// IViewUpdater updater is hooked into gameloop, gets update calls each frame and calls Render function inside IViews on frame update. Register and Deregister views that need to be updated.
/// IBankInterface bank small bank bridge class between bank app and gambling app, holds user loan amount.
/// UserInput userInput small input class that returns user input.
/// ViewRegistry viewRegistry data model that holds the different IViews.
/// </param>
/// <returns></returns>
public class SlotMachineController(
    SlotMachine machine,
    IViewUpdater updater,
    IBankInterface bank,
    UserInput userInput,
    ViewRegistry viewRegistry)
{
    private SpinResult? _spinResults;

    /// <summary>
    /// Entry function to run the gambling game.
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public IBankInterface PullTheSack() => RunSession();

    /// <summary>
    /// runs the router loop for gameplay and view updates. Exit on GamePhase exiting.
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    private IBankInterface RunSession()
    {
        ClearAllViews();
        machine.StartGame();

        ServeViewToUpdater(ViewType.SplashView);

        while (machine.State.GamePhase != GamePhase.Exiting)
        {
            StateControl(machine.State);
        }

        return bank;
    }

    /// <summary>
    /// state machine that routes the views and gameplay logic based on current gamephase.
    /// </summary>
    /// <param name="">
    /// GameState gameState represents current gamestate from the slotmachine, used to direct gameplay data.
    /// </param>
    /// <returns></returns>
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

                if (userInput.TryReadKeyCode(ConsoleKey.Spacebar))
                {
                    if (view.ViewState == ViewState.Complete)
                    {
                        bank.SetLoanAmount(423424);
                        machine.EndGame();
                    }
                }
                break;

            case GamePhase.Exiting:
                RemoveViewFromUpdater(ViewType.ResultView);
                break;
        }
    }

    /// <summary>
    /// Register IViewUpdater with a IView that will be updated each game loop.
    /// </summary>
    /// <param name="">
    /// ViewType view used to fetch IView from registry and update the IViewUpdater with that IView.
    /// </param>
    /// <returns></returns>
    private void ServeViewToUpdater(ViewType view) => updater.Register(viewRegistry.GetView(view));

    /// <summary>
    /// Removes IView from IViewUpdater and call reset function on that IView.
    /// </summary>
    /// <param name="">
    /// ViewType viewType used to fetch IView from registry to deregister from IVewUpdater
    /// </param>
    /// <returns></returns>
    private void RemoveViewFromUpdater(ViewType view)
    {
        IView v = viewRegistry.GetView(view);
        updater.DeRegister(v);
        v.ResetView();//Debatable if reset should be called here or if we should add a DeRegister function and let the IView decide if they want to reset on DeRegister. Currently we do want that for all of them, so it works here.
    }
    /// <summary>
    /// Deregisters all Iviews currently in IViewUpdater
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    private void ClearAllViews()
    {
        foreach (IView view in viewRegistry.AllViews)
        {
            updater.DeRegister(view);
        }
    }
}
