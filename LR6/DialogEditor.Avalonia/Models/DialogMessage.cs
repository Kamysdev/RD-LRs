using System.Collections.ObjectModel;

namespace DialogEditor.Avalonia.Models;

public sealed class DialogMessage
{
    public long MessageId { get; set; } = -1;

    public string Text { get; set; } = string.Empty;

    public ObservableCollection<DialogAnswer> Answers { get; } = new();
}
