namespace BankApp.Pull_The_Sack.View;

using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;
using BankApp.Pull_The_Sack.Interface;
using BankApp.Pull_The_Sack.View.Configs;

/// <summary>Parent layout for the main game screen; owns the child views.</summary>
public class GameView(GameViewConfig config, IReadOnlyList<ReelView> reels) : IView
{

    public bool HasBeenServed { get; set; } = false;
    public GameState GameState { get; set; }
    public ViewState ViewState { get; set; } = ViewState.Idle;

    private int BlockWidth;


    public void ResetView()
    {
        ViewState = ViewState.Idle;
    }

    public void Render(IConsole console, double dt)
    {
        console.Clear();

        if (ViewState == ViewState.Idle)
        {
            ViewState = ViewState.Animating;
            BlockWidth = reels.Count * config.ReelWidth + (reels.Count - 1) * config.ReelGap; //thank you deepseek
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
    }

    private void RenderSpinning(IConsole console, double dt)
    {
        int reelY;
        //calculate left reel top pos
        int leftReelX = Math.Max(0, (console.GetTerminalWidth() - BlockWidth / 2) / 2);
        
        for (int i = 1; i <= config.ReelRows; i++)
        {
            reelY = CalculateReelHeight(i);
            console.SetCursor(leftReelX, reelY);

            if (i == 2)
                console.SetConsoleColor(ConsoleColor.Yellow);
            else
                console.SetConsoleColor(ConsoleColor.White);

            reels[0].Render(console, dt); //pass in console after we update the cursor position. ReelView is only responsible for drawing the symbol. Not for placement, that is controller from gameview.
        }

        //calculate middle reel top pos
        int middleReelX = leftReelX + config.ReelWidth + config.ReelGap;
        for (int i = 1; i <= config.ReelRows; i++)
        {
            reelY = CalculateReelHeight(i);
            console.SetCursor(middleReelX, reelY);

            if (i == 2)
                console.SetConsoleColor(ConsoleColor.Yellow);
            else
                console.SetConsoleColor(ConsoleColor.White);

            reels[1].Render(console, dt);
        }

        console.SetConsoleColor(ConsoleColor.White);

        //calculate right reel top pos
        int righteReelX = leftReelX + ((config.ReelWidth + config.ReelGap) * 2);
        for (int i = 1; i <= config.ReelRows; i++)
        {
            reelY = CalculateReelHeight(i);
            console.SetCursor(righteReelX, reelY);

            if (i == 2)
                console.SetConsoleColor(ConsoleColor.Yellow);
            else
                console.SetConsoleColor(ConsoleColor.White);

            reels[2].Render(console, dt);
        }
    }

    private int CalculateReelHeight(int index) => config.ReelTop + (1 + config.ReelGap) * index;

}