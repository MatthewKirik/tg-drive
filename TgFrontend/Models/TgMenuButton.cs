namespace TgFrontend.Models;

public record TgMenuButton
{
    public string Text { get; init; } = default!;
    public ButtonCallbackHandler Handler { get; init; } = default!;
    public IEnumerable<string> Args { get; init; } = default!;

    public TgMenuButton(
        string text,
        ButtonCallbackHandler handler,
        params object[] args)
    {
        Handler = handler;
        Text = text;
        Args = args.Select(x => x.ToString()!)
            .ToList();
    }
}
