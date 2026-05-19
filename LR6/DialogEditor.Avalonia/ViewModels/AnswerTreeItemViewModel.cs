using DialogEditor.Avalonia.Models;

namespace DialogEditor.Avalonia.ViewModels;

public sealed class AnswerTreeItemViewModel : DialogTreeItemViewModel
{
    public AnswerTreeItemViewModel(DialogMessage owner, DialogAnswer answer)
    {
        Owner = owner;
        Answer = answer;
    }

    public DialogMessage Owner { get; }

    public DialogAnswer Answer { get; }

    public override string DisplayText
    {
        get
        {
            var linkText = Answer.LinkedMessageId >= 0 ? $" -> {Answer.LinkedMessageId}" : string.Empty;
            return $"{Answer.AnswerId}. {Shorten(Answer.Text)}{linkText}";
        }
    }

    private static string Shorten(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "(пустой ответ)";
        }

        text = text.ReplaceLineEndings(" ");
        return text.Length <= 36 ? text : text[..36] + "...";
    }
}
