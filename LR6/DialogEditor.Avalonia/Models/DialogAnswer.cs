namespace DialogEditor.Avalonia.Models;

public sealed class DialogAnswer
{
    public long AnswerId { get; set; } = -1;

    public long LinkedMessageId { get; set; } = -1;

    public string Text { get; set; } = string.Empty;

    public string Action { get; set; } = DialogActions.None;
}
