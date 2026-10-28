namespace BankApp.Pull_The_Sack;

using BankApp.Pull_The_Sack.API;
using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;
using BankApp.Pull_The_Sack.Interface;
using BankApp.Pull_The_Sack.IO;
using BankApp.Pull_The_Sack.Service;
using BankApp.Pull_The_Sack.View;
using BankApp.Pull_The_Sack.View.Configs;

/// <summary>
/// Composition root for the Pull The Sack mini-game. Wires up the model,
/// views, controller, and bank integration, then runs a session.
/// </summary>
public class Orchestrator
{
    /// <summary>Builds the object graph and runs a single game session.</summary>
    public void Start(decimal loanAmount)
    {
        // --- Infrastructure ---
        IConsole console = new LockedConsole();
        IViewUpdater updater = new ViewUpdater(console);
        UserInput userInput = new UserInput(console);
        IBankInterface bank = new BankStub(loanAmount);

        // --- RNG + symbol catalog ---
        IRng rng = new Rng();
        SymbolCatalog catalog = new(new Dictionary<int, SlotSymbol>
        {
            [0] = SlotSymbol.Sackpot,
            [1] = SlotSymbol.Awesome,
            [2] = SlotSymbol.Coin,
            [3] = SlotSymbol.Star,
            [4] = SlotSymbol.Bar,
            [5] = SlotSymbol.Stash,
            [6] = SlotSymbol.Ampersand,
        });

        SymbolCatalog.SymbolGraphic = new Dictionary<SlotSymbol, string>
        {
            [SlotSymbol.Sackpot] = "$",
            [SlotSymbol.Awesome] = "A",
            [SlotSymbol.Coin] = "@",
            [SlotSymbol.Star] = "*",
            [SlotSymbol.Bar] = "=",
            [SlotSymbol.Stash] = "#",
            [SlotSymbol.Ampersand] = "&",
        };

        // --- Reels + payout ---
        IReadOnlyList<Reel> reels =
        [
            new Reel(catalog, rng),
            new Reel(catalog, rng),
            new Reel(catalog, rng),
        ];

        IPayoutEvaluator evaluator = new PayoutEvaluator(new PayoutTable());

        // --- Game loop + model ---
        GameLoop gameLoop = new(updater, framesPerSecond: 2);
        SlotMachine machine = new(reels, evaluator, loanAmount, gameLoop);

        // --- Views ---
        SplashView splash = new(new SplashViewConfig());

        List<ReelView> reels_tmp = [];

        for (int i = 0; i < 3; i++)
        {
            reels_tmp.Add(new ReelView([.. SymbolCatalog.SymbolGraphic.Values], 3, 5));
        }
        
        IReadOnlyList<ReelView> reelViews = [.. reels_tmp];

        BalanceView balance = new(loanAmount);

        GameView game = new(new GameViewConfig { ReelCols = reels.Count }, reelViews);
        // ResultView intentionally not wired (testing splash only).

        // --- Registry ---
        ViewRegistry registry = new();
        registry.AddView(ViewType.SplashView, splash);
        registry.AddView(ViewType.GameView, game);

        // --- Controller + run ---
        SlotMachineController controller =
            new(machine, updater, bank, userInput, registry);
        controller.PullTheSack();
    }
}