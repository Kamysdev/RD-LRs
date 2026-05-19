using System.Collections.ObjectModel;
using System.Linq;

namespace DialogEditor.Avalonia.Models;

public sealed class DialogGraph
{
    public ObservableCollection<DialogMessage> Messages { get; } = new();

    public long CurrentUid { get; private set; }

    public long GenerateUid()
    {
        CurrentUid++;
        return CurrentUid;
    }

    public void SetCurrentUid(long uid)
    {
        CurrentUid = uid;
    }

    public DialogMessage? AddMessage(string text)
    {
        var message = new DialogMessage
        {
            MessageId = GenerateUid(),
            Text = text,
        };

        Messages.Add(message);
        return message;
    }

    public DialogAnswer AddAnswer(DialogMessage message, string text, string action)
    {
        var answer = new DialogAnswer
        {
            AnswerId = GenerateUid(),
            Text = text,
            Action = action,
        };

        message.Answers.Add(answer);
        return answer;
    }

    public void RemoveMessage(DialogMessage message)
    {
        Messages.Remove(message);

        foreach (var answer in Messages.SelectMany(current => current.Answers))
        {
            if (answer.LinkedMessageId == message.MessageId)
            {
                answer.LinkedMessageId = -1;
            }
        }
    }

    public void RemoveAnswer(DialogAnswer answer)
    {
        var owner = Messages.FirstOrDefault(message => message.Answers.Contains(answer));
        owner?.Answers.Remove(answer);
    }

    public void Clear()
    {
        Messages.Clear();
        CurrentUid = 0;
    }
}
