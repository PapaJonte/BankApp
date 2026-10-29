namespace BankApp.Pull_The_Sack.View;

using BankApp.Pull_The_Sack.Data_Model;
using BankApp.Pull_The_Sack.Enum;
using BankApp.Pull_The_Sack.Interface;

/// <summary>Renders a single <see cref="Reel"/> and its spin animation.</summary>
public class ReelView(string[] SlotSymbols, int RowCount, int Width) : IView
{
    public bool HasBeenServed { get; set; } = false;
    public GameState GameState { get; set; }
    public ViewState ViewState { get; set; } = ViewState.Idle;
    public Action? OnResolveComplete { get; set; }

    private bool resolveComplete;
    private float sequenceSpeed;
    private float sequenceSpeedDecay;
    private int framesSinceLastUpdate;
    private int symbolIndex = 0; //symbol index for top reel view. then we add current render iteration to it
    private int reelSlot = 0;//which reel slot is currently being rendered, 1=>RowCount
    private readonly Random rnd = new();
    private bool runningResolveSequence;


    public void ResetView()
    {
        ViewState = ViewState.Idle;
    }

    public void Render(IConsole console, double dt)
    {
        if (ViewState == ViewState.Idle)
        {
            ViewState = ViewState.Animating;
            symbolIndex = PickStartSymbolIndex();
        }

        int x = console.Left;
        int y = console.Top;

        string top = "┌" + new string('─', Width - 2) + "┐";

        console.WriteAtCursor(top);

        console.SetCursor(x, y + 1);

        console.WriteAtCursor("|");

        console.SetCursor(x + 1 + (Width - 2 - 1) / 2, y + 1);

        console.WriteAtCursor(SlotSymbols[(symbolIndex + reelSlot) % SlotSymbols.Length]);

        console.SetCursor(x + Width - 1, y + 1);

        console.WriteAtCursor("│");

        string bottom = "└" + new string('─', Width - 2) + "┘";

        console.SetCursor(x, y + 2);

        console.WriteAtCursor(bottom);

        ++reelSlot;

        if (reelSlot == RowCount)
        {
            reelSlot = 0;

            if (runningResolveSequence)
            {
                if(sequenceSpeed <= 0)
                {
                    if (!resolveComplete)
                    {
                        resolveComplete = true;
                        OnResolveComplete?.Invoke();
                    }

                    return;
                }
                if (framesSinceLastUpdate * sequenceSpeed < 1)
                {
                    ++framesSinceLastUpdate;
                }
                else
                {
                    symbolIndex = WrapAround(--symbolIndex, SlotSymbols.Length - 1);
                    sequenceSpeed = Math.Clamp(sequenceSpeed - sequenceSpeedDecay, 0, float.MaxValue);
                    framesSinceLastUpdate = 1;
                }
            }
            else
            {
                symbolIndex = WrapAround(--symbolIndex, SlotSymbols.Length - 1);
            }
        }
    }

    private int WrapAround(int val, int maxVal)
    {
        if (val < 0)
            val = maxVal;
        else if (val > maxVal)
            val = 0;

        return val;
    }

    private int PickStartSymbolIndex() => rnd.Next(0, SlotSymbols.Length - 1);

    public void Target(string target, int sequenceLenght, float sequenceSpeed, float sequenceSpeedDecay)
    {
        this.sequenceSpeed = sequenceSpeed;
        this.sequenceSpeedDecay = sequenceSpeedDecay;
        framesSinceLastUpdate = 1;
        string[] temp = new string[sequenceLenght * RowCount];
        int currentIndex = symbolIndex;

        for (int i = 0; i < sequenceLenght * RowCount; i++)
        {
            if (i == 0)
            {
                temp[i] = target;
            }
            else
            {
                temp[i] = SlotSymbols[currentIndex];
                currentIndex = WrapAround(--currentIndex, SlotSymbols.Length - 1);
            }
        }
        symbolIndex = sequenceLenght -1;
        SlotSymbols = temp;
        runningResolveSequence = true;
    }

}