namespace BankApp.Pull_The_Sack.View;

using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;
using BankApp.Pull_The_Sack.Interface;
using BankApp.Pull_The_Sack.View.Configs;

/// <summary>Branding / intro screen shown before a game starts.</summary>
public class SplashView(SplashViewConfig config) : IView
{

    public bool HasBeenServed { get; set; } = false;
    public GameState GameState { get; set; }
    public ViewState ViewState { get; set; } = ViewState.Idle;


    private int currentFrame = 0;


    public void Render(IConsole console, double dt)
    {
        console.Clear();
        
        string frame = config.Frames[currentFrame % config.Frames.Count];

        console.WriteCentered(config.SponsorText);
        console.WriteCentered(frame);

        ++currentFrame;

        if (currentFrame < 4)
        {
            ViewState = ViewState.Animating;
        }
        else
        {
            ViewState = ViewState.Complete;
        }
            
    }

    public void ResetView()
    {
        currentFrame = 0;
        HasBeenServed = false;
    }


}


