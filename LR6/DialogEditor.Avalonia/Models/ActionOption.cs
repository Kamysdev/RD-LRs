namespace DialogEditor.Avalonia.Models;

public sealed class ActionOption
{
    public ActionOption(string value, string displayName)
    {
        Value = value;
        DisplayName = displayName;
    }

    public string Value { get; }

    public string DisplayName { get; }
}
