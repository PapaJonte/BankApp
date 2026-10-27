using System.Diagnostics.SymbolStore;
using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;
using BankApp.Pull_The_Sack.Interface;

namespace BankApp.Pull_The_Sack.Service;

/// <summary>
/// Core game model. Owns the reels, evaluates spins, and tracks game state.
/// Free of any presentation/console concerns.
/// </summary>
public class SlotMachine(IReadOnlyList<Reel> reels, IPayoutEvaluator evaluator, decimal startingBalance, GameLoop gameLoop)
{

    /// <summary>Current immutable state snapshot.</summary>
    public GameState State { get; private set; } = new GameState(GamePhase.Splash, startingBalance, 0, null);

    public void StartGame()
    {
        UpdateGamePhase(GamePhase.Splash);
        gameLoop.StartGameLoopAsync();
    }

    public void EndGame()
    {
        UpdateGamePhase(GamePhase.Exiting);
        gameLoop.StopGameLoopAsync();
    }

    /// <summary>Spins all reels and produces a result. Does not mutate balance yet.</summary>
    public void Spin()
    {
        UpdateGamePhase(GamePhase.Spinning);
        foreach (var reel in reels)
            reel.Spin();
    }

    public void ResolveSpinResult()
    {
        UpdateGamePhase(GamePhase.Resolving);
        IReadOnlyList<SlotSymbol> reelSymbols = [.. reels.Select(x => x.CurrentSymbol)];//did not know C# had spread
        var payoutMultipliler = evaluator.Evaluate(reelSymbols);

        List<SlotSymbol> symbols = [.. reels.Select(x => x.CurrentSymbol)];

        SpinResult result = new(symbols, payoutMultipliler);

        State = State with {SpinResult = result };
    }

    private void UpdateGamePhase(GamePhase gamePhase)
    {
        State = State with { GamePhase = gamePhase };
    }

    /// <summary>Applies a spin result to the current balance.</summary>
    public void Apply()
    {
        UpdateGamePhase(GamePhase.Result);
        //fetch result from gamestate here
    }
}