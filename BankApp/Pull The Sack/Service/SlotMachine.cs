using System.Diagnostics.SymbolStore;
using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;
using BankApp.Pull_The_Sack.Interface;

namespace BankApp.Pull_The_Sack.Service;

/// <summary>
/// Core game model. Owns the reels, evaluates spins, and tracks game state.
/// </summary>
public class SlotMachine(IReadOnlyList<Reel> reels, IPayoutEvaluator evaluator, decimal startingBalance, GameLoop gameLoop)
{

    public GameState State { get; private set; } = new GameState(GamePhase.Splash, startingBalance, 0, null);

    /// <summary>
    /// Start the game, fires up the game loop and updates current game phase
    /// </summary>
    public void StartGame()
    {
        UpdateGamePhase(GamePhase.Splash);
        gameLoop.StartGameLoopAsync();
    }

    /// <summary>
    /// Ends the game, stops the game loop and updates the current game phase
    /// </summary>
    public void EndGame()
    {
        UpdateGamePhase(GamePhase.Exiting);
        gameLoop.StopGameLoopAsync();
    }

    /// <summary>
    /// Loops through each reel and spin them, updates current game phase
    /// </summary>
    public void Spin()
    {
        UpdateGamePhase(GamePhase.Spinning);
        foreach (var reel in reels)
            reel.Spin();
    }

    /// <summary>
    /// Updates current gamephase to resolving and get result for reel spins. Add result to gamestate
    /// </summary>
    public void ResolveSpinResult()
    {
        IReadOnlyList<SlotSymbol> reelSymbols = [.. reels.Select(x => x.CurrentSymbol)];//did not know C# had spread
        var payoutMultipliler = evaluator.Evaluate(reelSymbols);

        List<SlotSymbol> symbols = [.. reels.Select(x => x.CurrentSymbol)];

        SpinResult result = new(symbols, payoutMultipliler);
        State = State with { SpinResult = result };
        UpdateGamePhase(GamePhase.Resolving);
    }

    /// <summary>
    /// Updates the game phase with passed in game phase
    /// </summary>
    /// <param name="gamePhase">the new gamephase</param>
    private void UpdateGamePhase(GamePhase gamePhase)
    {
        State = State with { GamePhase = gamePhase };
    }

    /// <summary>
    /// updates gamephase to result
    /// </summary>
    public void Apply()
    {
        UpdateGamePhase(GamePhase.Result);
    }
}