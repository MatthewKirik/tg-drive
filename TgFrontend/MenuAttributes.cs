namespace TgFrontend;

[AttributeUsage(AttributeTargets.Method)]
internal class TgButtonCallbackAttribute : Attribute
{
    public string ButtonId { get; }

    public TgButtonCallbackAttribute(string buttonId)
    {
        ButtonId = buttonId;
    }
}

[AttributeUsage(AttributeTargets.Method)]
internal class TgMessageResponseAttribute : Attribute
{
    public string ButtonId { get; }

    public TgMessageResponseAttribute(string buttonId)
    {
        ButtonId = buttonId;
    }
}

[AttributeUsage(AttributeTargets.Class)]
internal class TgMenuAttribute : Attribute
{
    public string MenuId { get; }

    public TgMenuAttribute(string menuId)
    {
        MenuId = menuId;
    }
}
