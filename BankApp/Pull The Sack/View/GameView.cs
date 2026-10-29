namespace BankApp.Pull_The_Sack.View;

using System.Collections.Concurrent;
using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;
using BankApp.Pull_The_Sack.Interface;
using BankApp.Pull_The_Sack.View.Configs;

/// <summary>Parent layout for the main game screen; owns the child views.</summary>
public class GameView : IView
{

    public bool HasBeenServed { get; set; } = false;
    public GameState GameState { get; set; }
    public ViewState ViewState { get; set; } = ViewState.Idle;

    private readonly GameViewConfig config;
    private readonly IReadOnlyList<ReelView> reels;
    private int BlockWidth;
    private bool resolved;
    private bool complete;
    private bool blinkGraphic;
    private int blinkCount = 0;
    private int targetBlinkCounts = 4;
    private ConsoleColor[] blinkColor;


    public GameView(GameViewConfig config, IReadOnlyList<ReelView> reels)
    {
        this.config = config;
        this.reels = reels;
        blinkColor = new ConsoleColor[2];
        blinkColor[0] = ConsoleColor.White;
        blinkColor[1] = ConsoleColor.Yellow;
    }

    public void ResetView()
    {
        ViewState = ViewState.Idle;
        resolved = false;
    }

    public void Render(IConsole console, double dt)
    {
        //lock the whole render logic instead of individual draw calls
        //this prevents flickering and is compatible to use with the
        using (console.Lock())
        {
            console.Clear();

            if (ViewState == ViewState.Idle)
            {
                ViewState = ViewState.Animating;
                BlockWidth = reels.Count * config.ReelWidth + (reels.Count - 1) * config.ReelGap; //thank you deepseek
            }

            if (GameState.GamePhase == GamePhase.Resolving && !resolved)
            {
                Resolve();
            }

            #region Draw top graphics
            console.SetConsoleColor(ConsoleColor.Yellow);
            console.WriteCentered(config.Banner);
            console.SetConsoleColor(ConsoleColor.White);
            console.WriteCentered(config.SponsorText);
            console.SetConsoleColor(ConsoleColor.Yellow);
            console.WriteCentered(config.TopArt);
            console.SetConsoleColor(ConsoleColor.White);
            #endregion

            RenderSpinning(console, dt);

            #region Draw bottom graphics
            console.SetConsoleColor(ConsoleColor.Yellow);
            console.WriteCentered("");//offset bottom graphic
            console.WriteCentered(config.BottomArt);
            #endregion

            if (complete)
            {
                string outcome = EvaluateCompleteText();
                console.WriteCentered(outcome);
                console.WriteCentered("");
                console.WriteCentered("Press space to continue");
                ViewState = ViewState.Complete;
            }

            //gameview register callbacks with reels for when they are done
            //calls reels to start blink functionality
            //once blink is complete another callback
            //let user continue
        }
    }

    private void RenderSpinning(IConsole console, double dt)
    {
        int reelY;
        //calculate left reel top pos
        int reelX = (console.GetTerminalWidth() - BlockWidth) / 2, mReelX;

        for (int r = 0; r < config.ReelCols; r++)
        {
            mReelX = reelX + ((config.ReelWidth + config.ReelGap) * r);
            for (int i = 1; i <= config.ReelRows; i++)
            {
                reelY = CalculateReelHeight(i);
                console.SetCursor(mReelX, reelY);

                if (i == 2)
                {
                    if (blinkGraphic)
                    {
                        ConsoleColor c = blinkColor[blinkCount % blinkColor.Length];
                        console.SetConsoleColor(c);
                    }
                    else
                    {
                        console.SetConsoleColor(ConsoleColor.Yellow);
                    }
                }
                else
                    console.SetConsoleColor(ConsoleColor.White);

                reels[r].Render(console, dt); /*pass in console after we update the cursor position. ReelView is only responsible for drawing the symbol. Not for placement, that is controller from gameview. 
                **Update** it is now a lil responsible for placement since its drawing its own box. But it's akin to GameView placing in worldspace/terminal space and reelview placeing in localspace, kind of.
                */
            }
        }

        if (blinkGraphic && blinkCount >= targetBlinkCounts)
        {
            blinkGraphic = false;
            complete = true;
        }
        else if (blinkGraphic)
        {
            ++blinkCount;
        }
    }


    /*
    Ok so this is a last minute fix because I didnt actually look up the rules for the slot machine -.-
    apparently the slot result is evaluated before you begin spinning and you should setup the reel spin offset from the start.
    But because I didn't do that, we now have to offset the reel target spins when we're already spinning. And I guess this is one way of doing so.. We just add extra buffer to spin symbols on the reels that run longer
    and let them run at each frame, determined by frame delta and current scroll speed for that reel.
    and then we slow down the scroll speed (skipping frames basically) when we approach the spin target.
    */
    private void Resolve()
    {
        if (GameState.SpinResult == null)
        {
            throw new NullReferenceException("Spin Result cannot be null!");
        }

        if (GameState.SpinResult.Symbols.Count != reels.Count)
        {
            throw new FormatException("Spin Result does not match reels count!");

        }

        string[] targetSymbols = new string[GameState.SpinResult.Symbols.Count];

        for (int i = 0; i < GameState.SpinResult.Symbols.Count; i++)
        {
            SlotSymbol symbl = GameState.SpinResult.Symbols[i];
            targetSymbols[i] = SymbolCatalog.GetSymbolGraphicNullCheck(symbl);
        }

        for (int i = 0; i < reels.Count; i++)
        {
            reels[i].Target(targetSymbols[i], config.ResolveSlowDownSteps + (i * 1), 1 + (i * .25f), .25f);
        }

        reels[reels.Count - 1].OnResolveComplete += OnReelResolveComplete; //listen for when last reel has displayed its target symbol
        resolved = true;
    }

    private void OnReelResolveComplete()
    {
        reels[reels.Count - 1].OnResolveComplete -= OnReelResolveComplete;
        blinkGraphic = true;
    }

    private string EvaluateCompleteText()
    {
        string outcome;

        if (GameState.SpinResult == null)
        {
            throw new NullReferenceException("SpinResult is null!");
        }

        if (GameState.SpinResult.PayoutMultiplier >= 5)
        {
            outcome = config.CompleteText_SackPot;
        }
        else if (GameState.SpinResult.PayoutMultiplier > 1)
        {
            outcome = config.CompleteText_Won;
        }
        else
        {
            outcome = config.CompleteText_Lost;
        }

        return outcome;
    }

    private int CalculateReelHeight(int index) => config.ReelTop + (1 + config.ReelGap) * index;

}