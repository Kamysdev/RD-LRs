using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using DialogEditor.Avalonia.Models;
using DialogEditor.Avalonia.Services;

namespace DialogEditor.Avalonia.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly DialogXmlSerializer _serializer = new();
    private DialogGraph _graph = new();
    private string _editorText = string.Empty;
    private string _rawActionText = DialogActions.None;
    private string _statusText = "Create the first message, then add answers, links and action clauses.";
    private DialogMessage? _selectedMessage;
    private DialogAnswer? _selectedAnswer;
    private bool _isSyncingRawAction;

    public MainWindowViewModel()
    {
        TreeItems = new ObservableCollection<MessageTreeItemViewModel>();
        ActionClauses = new ObservableCollection<ActionClauseViewModel>();
    }

    public ObservableCollection<MessageTreeItemViewModel> TreeItems { get; }

    public ObservableCollection<ActionClauseViewModel> ActionClauses { get; }

    public string EditorText
    {
        get => _editorText;
        set => SetProperty(ref _editorText, value);
    }

    public string RawActionText
    {
        get => _rawActionText;
        set => SetProperty(ref _rawActionText, value);
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    public string SelectedActionDisplay => DialogActions.GetDisplayName(RawActionText);

    public long SelectedMessageId => _selectedMessage?.MessageId ?? -1;

    public long SelectedAnswerId => _selectedAnswer?.AnswerId ?? -1;

    public long LinkedMessageId => _selectedAnswer?.LinkedMessageId ?? -1;

    public bool HasSelectedMessage => _selectedMessage is not null;

    public bool HasSelectedAnswer => _selectedAnswer is not null;

    public bool CanLinkAnswer => HasSelectedMessage && HasSelectedAnswer;

    public void SelectTreeItem(object? item)
    {
        switch (item)
        {
            case MessageTreeItemViewModel messageItem:
                _selectedMessage = messageItem.Message;
                _selectedAnswer = null;
                EditorText = messageItem.Message.Text;
                ClearActionEditor();
                StatusText = $"Message {messageItem.Message.MessageId} selected.";
                OnSelectionChanged();
                break;

            case AnswerTreeItemViewModel answerItem:
                _selectedAnswer = answerItem.Answer;
                EditorText = answerItem.Answer.Text;
                LoadActionEditor(answerItem.Answer.Action);
                StatusText = $"Answer {answerItem.Answer.AnswerId} of message {answerItem.Owner.MessageId} selected.";
                OnSelectionChanged();
                break;
        }
    }

    public void AddMessage()
    {
        var message = _graph.AddMessage(GetDefaultMessageText());
        RebuildTree();
        _selectedMessage = message;
        _selectedAnswer = null;
        EditorText = message!.Text;
        ClearActionEditor();
        StatusText = $"Message {message.MessageId} added.";
        OnSelectionChanged();
    }

    public void AddAnswer()
    {
        if (_selectedMessage is null)
        {
            StatusText = "Select a message first.";
            return;
        }

        var answer = _graph.AddAnswer(_selectedMessage, GetDefaultAnswerText(), GetNormalizedActionText());
        RebuildTree();
        _selectedAnswer = answer;
        EditorText = answer.Text;
        LoadActionEditor(answer.Action);
        StatusText = $"Answer {answer.AnswerId} added to message {_selectedMessage.MessageId}.";
        OnSelectionChanged();
    }

    public void UpdateSelectedItem()
    {
        if (_selectedAnswer is not null)
        {
            _selectedAnswer.Text = NormalizeEditorText();
            _selectedAnswer.Action = GetNormalizedActionText();
            LoadActionEditor(_selectedAnswer.Action);
            RebuildTree();
            StatusText = $"Answer {_selectedAnswer.AnswerId} updated.";
            OnSelectionChanged();
            return;
        }

        if (_selectedMessage is not null)
        {
            _selectedMessage.Text = NormalizeEditorText();
            RebuildTree();
            StatusText = $"Message {_selectedMessage.MessageId} updated.";
            OnSelectionChanged();
            return;
        }

        StatusText = "Select a message or answer to update.";
    }

    public void DeleteSelectedItem()
    {
        if (_selectedAnswer is not null)
        {
            var removedId = _selectedAnswer.AnswerId;
            _graph.RemoveAnswer(_selectedAnswer);
            _selectedAnswer = null;
            RebuildTree();
            ClearActionEditor();
            StatusText = $"Answer {removedId} removed.";
            OnSelectionChanged();
            return;
        }

        if (_selectedMessage is not null)
        {
            var removedId = _selectedMessage.MessageId;
            _graph.RemoveMessage(_selectedMessage);
            _selectedMessage = null;
            _selectedAnswer = null;
            RebuildTree();
            ClearActionEditor();
            StatusText = $"Message {removedId} removed.";
            OnSelectionChanged();
            return;
        }

        StatusText = "Nothing is selected for deletion.";
    }

    public void LinkSelectedAnswerToSelectedMessage()
    {
        if (_selectedAnswer is null)
        {
            StatusText = "Select an answer first.";
            return;
        }

        if (_selectedMessage is null)
        {
            StatusText = "Select a target message first.";
            return;
        }

        _selectedAnswer.LinkedMessageId = _selectedMessage.MessageId;
        RebuildTree();
        StatusText = $"Answer {_selectedAnswer.AnswerId} linked to message {_selectedMessage.MessageId}.";
        OnSelectionChanged();
    }

    public void AddIncreaseClause()
    {
        AddClause(CreateClause(DialogActions.IncreaseStat));
        StatusText = "Increase stat clause added.";
    }

    public void AddCheckClause()
    {
        AddClause(CreateClause(DialogActions.CheckStat));
        StatusText = "Stat check clause added.";
    }

    public void AddOpenDoorClause()
    {
        AddClause(CreateClause(DialogActions.OpenDoor));
        StatusText = "Open door clause added.";
    }

    public void AddEndDialogClause()
    {
        AddClause(CreateClause(DialogActions.EndDialog));
        StatusText = "End dialogue clause added.";
    }

    public void AddCustomClause()
    {
        AddClause(CreateClause(DialogActions.Custom));
        StatusText = "Custom clause added.";
    }

    public void RemoveClause(ActionClauseViewModel? clause)
    {
        if (clause is null)
        {
            return;
        }

        clause.PropertyChanged -= ClausePropertyChanged;
        ActionClauses.Remove(clause);
        SyncRawActionFromClauses();
        StatusText = "Clause removed.";
    }

    public void ParseRawAction()
    {
        LoadActionEditor(GetNormalizedActionText());
        StatusText = "Raw action parsed into clauses.";
    }

    public void ResetActions()
    {
        ClearActionEditor();
        StatusText = "Action list reset to none.";
    }

    public void Clear()
    {
        _graph.Clear();
        _selectedMessage = null;
        _selectedAnswer = null;
        EditorText = string.Empty;
        TreeItems.Clear();
        ClearActionEditor();
        StatusText = "Dialogue cleared.";
        OnSelectionChanged();
    }

    public void Save(Stream output)
    {
        _serializer.Save(_graph, output);
        StatusText = $"XML saved. Messages: {_graph.Messages.Count}.";
    }

    public void Load(Stream input)
    {
        _graph = _serializer.Load(input);
        _selectedMessage = null;
        _selectedAnswer = null;
        EditorText = string.Empty;
        RebuildTree();
        ClearActionEditor();
        StatusText = $"XML loaded. Messages: {_graph.Messages.Count}.";
        OnSelectionChanged();
    }

    private void RebuildTree()
    {
        TreeItems.Clear();

        foreach (var message in _graph.Messages)
        {
            var messageItem = new MessageTreeItemViewModel(message);
            foreach (var answer in message.Answers)
            {
                messageItem.Children.Add(new AnswerTreeItemViewModel(message, answer));
            }

            TreeItems.Add(messageItem);
        }
    }

    private void OnSelectionChanged()
    {
        OnPropertyChanged(nameof(SelectedMessageId));
        OnPropertyChanged(nameof(SelectedAnswerId));
        OnPropertyChanged(nameof(LinkedMessageId));
        OnPropertyChanged(nameof(HasSelectedMessage));
        OnPropertyChanged(nameof(HasSelectedAnswer));
        OnPropertyChanged(nameof(CanLinkAnswer));
        OnPropertyChanged(nameof(SelectedActionDisplay));
    }

    private void ClearActionEditor()
    {
        foreach (var clause in ActionClauses)
        {
            clause.PropertyChanged -= ClausePropertyChanged;
        }

        ActionClauses.Clear();
        _isSyncingRawAction = true;
        RawActionText = DialogActions.None;
        _isSyncingRawAction = false;
        OnPropertyChanged(nameof(SelectedActionDisplay));
    }

    private void LoadActionEditor(string actionText)
    {
        foreach (var clause in ActionClauses)
        {
            clause.PropertyChanged -= ClausePropertyChanged;
        }

        ActionClauses.Clear();

        var normalizedAction = DialogActions.CanonicalizeActionString(actionText);
        if (!string.Equals(normalizedAction, DialogActions.None, StringComparison.OrdinalIgnoreCase))
        {
            foreach (var token in normalizedAction.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                AddClauseInternal(ActionClauseViewModel.FromToken(token));
            }
        }

        _isSyncingRawAction = true;
        RawActionText = normalizedAction;
        _isSyncingRawAction = false;
        OnPropertyChanged(nameof(SelectedActionDisplay));
    }

    private void AddClause(ActionClauseViewModel clause)
    {
        AddClauseInternal(clause);
        SyncRawActionFromClauses();
    }

    private void AddClauseInternal(ActionClauseViewModel clause)
    {
        clause.PropertyChanged += ClausePropertyChanged;
        ActionClauses.Add(clause);
    }

    private void ClausePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ActionClauseViewModel.SelectedType)
            or nameof(ActionClauseViewModel.SelectedStat)
            or nameof(ActionClauseViewModel.SelectedOperator)
            or nameof(ActionClauseViewModel.ValueText)
            or nameof(ActionClauseViewModel.CustomToken))
        {
            SyncRawActionFromClauses();
        }
    }

    private void SyncRawActionFromClauses()
    {
        if (_isSyncingRawAction)
        {
            return;
        }

        _isSyncingRawAction = true;
        RawActionText = ActionClauses.Count == 0
            ? DialogActions.None
            : string.Join("; ", ActionClauses.Select(clause => clause.BuildToken()));
        _isSyncingRawAction = false;
        OnPropertyChanged(nameof(SelectedActionDisplay));
    }

    private string NormalizeEditorText()
    {
        return string.IsNullOrWhiteSpace(EditorText) ? "(empty)" : EditorText.Trim();
    }

    private string GetDefaultMessageText()
    {
        return $"Message {_graph.Messages.Count + 1}";
    }

    private string GetDefaultAnswerText()
    {
        if (_selectedMessage is null)
        {
            return "Answer";
        }

        return $"Answer {_selectedMessage.Answers.Count + 1} for message {_selectedMessage.MessageId}";
    }

    private string GetNormalizedActionText()
    {
        return DialogActions.CanonicalizeActionString(RawActionText);
    }

    private static ActionClauseViewModel CreateClause(string type)
    {
        return ActionClauseViewModel.FromToken(type switch
        {
            DialogActions.IncreaseStat => "stat_inc:strength:1",
            DialogActions.CheckStat => "stat_check:strength:>=:2",
            DialogActions.OpenDoor => DialogActions.OpenDoor,
            DialogActions.EndDialog => DialogActions.EndDialog,
            DialogActions.Custom => "custom_token",
            _ => DialogActions.None,
        });
    }
}
