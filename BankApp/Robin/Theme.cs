using Spectre.Console;

namespace BankApp.Robin
{
    internal class Theme
    {
        Style style = new(
    foreground: Color.Blue,
    background: Color.White,
    decoration: Decoration.Bold);

        public void Markup()
        {
            AnsiConsole.Write(new Markup("Welcome", style));

        }

    }
}
