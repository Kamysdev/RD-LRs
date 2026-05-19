using System.Collections.Generic;
using System.Linq;

namespace LR6.Dialogues
{
    [System.Serializable]
    public sealed class DialogueAnswer
    {
        public long AnswerId = -1;
        public long LinkedMessageId = -1;
        public string Text = string.Empty;
        public string Action = "none";
    }

    [System.Serializable]
    public sealed class DialogueMessage
    {
        public long MessageId = -1;
        public string Text = string.Empty;
        public List<DialogueAnswer> Answers = new();
    }

    public sealed class DialogueData
    {
        private readonly List<DialogueMessage> messages = new();

        public IReadOnlyList<DialogueMessage> Messages => messages;

        public long CurrentUid { get; private set; }

        public DialogueMessage? SelectedMessage { get; private set; }

        public DialogueAnswer? SelectedAnswer { get; private set; }

        public void Clear()
        {
            messages.Clear();
            CurrentUid = 0;
            SelectedMessage = null;
            SelectedAnswer = null;
        }

        public void SetCurrentUid(long uid)
        {
            CurrentUid = uid;
        }

        public void LoadMessage(DialogueMessage message)
        {
            messages.Add(message);
        }

        public DialogueMessage? FindMessage(long messageId)
        {
            return messages.FirstOrDefault(message => message.MessageId == messageId);
        }

        public string SelectMessage(long messageId)
        {
            SelectedMessage = FindMessage(messageId);
            SelectedAnswer = null;
            return SelectedMessage?.Text ?? string.Empty;
        }

        public void SelectAnswer(long messageId, long answerId)
        {
            SelectedMessage = FindMessage(messageId);
            SelectedAnswer = SelectedMessage?.Answers.FirstOrDefault(answer => answer.AnswerId == answerId);
        }

        public IReadOnlyList<DialogueAnswer> GetAnswers()
        {
            return SelectedMessage?.Answers ?? (IReadOnlyList<DialogueAnswer>)System.Array.Empty<DialogueAnswer>();
        }
    }
}
