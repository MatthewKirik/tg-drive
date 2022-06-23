namespace TgGateway.Models;

public record TgButton(string Text, string Callback, IEnumerable<string> args);
