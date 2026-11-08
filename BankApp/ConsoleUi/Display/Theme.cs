using Spectre.Console;

namespace BankApp.Robin
{
    internal class Theme
    {
        Style style = new(
    foreground: Color.Blue,
    background: Color.White,
    decoration: Decoration.Bold);

        public void Markup(string input)
        {
            AnsiConsole.Write(new Markup(input, style));
        }

    }
}
