using System.Collections.ObjectModel;
using DialogEditor.Avalonia.Models;

namespace DialogEditor.Avalonia.ViewModels;

public sealed class MessageTreeItemViewModel : DialogTreeItemViewModel
{
    public MessageTreeItemViewModel(DialogMessage message)
    {
        Message = message;
        Children = new ObservableCollection<DialogTreeItemViewModel>();
    }

    public DialogMessage Message { get; }

    public ObservableCollection<DialogTreeItemViewModel> Children { get; }

    public override string DisplayText => $"{Message.MessageId}. {Shorten(Message.Text)}";

    private static string Shorten(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "(пустое сообщение)";
        }

        text = text.ReplaceLineEndings(" ");
        return text.Length <= 40 ? text : text[..40] + "...";
    }
}
