using System;
using System.Collections.Generic;
using DialogEditor.Avalonia.Models;

namespace DialogEditor.Avalonia.ViewModels;

public sealed class ActionClauseViewModel : ViewModelBase
{
    private ActionOption? _selectedType;
    private ActionOption? _selectedStat;
    private ActionOption? _selectedOperator;
    private string _valueText = "1";
    private string _customToken = string.Empty;

    public ActionClauseViewModel()
    {
        ClauseTypes = DialogActions.ClauseTypes;
        StatOptions = DialogActions.Stats;
        OperatorOptions = DialogActions.CompareOperators;

        _selectedType = ClauseTypes[0];
        _selectedStat = StatOptions[0];
        _selectedOperator = OperatorOptions[0];
    }

    public IReadOnlyList<ActionOption> ClauseTypes { get; }

    public IReadOnlyList<ActionOption> StatOptions { get; }

    public IReadOnlyList<ActionOption> OperatorOptions { get; }

    public ActionOption? SelectedType
    {
        get => _selectedType;
        set
        {
            if (SetProperty(ref _selectedType, value))
            {
                OnPropertyChanged(nameof(ShowsStat));
                OnPropertyChanged(nameof(ShowsOperator));
                OnPropertyChanged(nameof(ShowsValue));
                OnPropertyChanged(nameof(ShowsCustomToken));
                OnPropertyChanged(nameof(PreviewText));
            }
        }
    }

    public ActionOption? SelectedStat
    {
        get => _selectedStat;
        set
        {
            if (SetProperty(ref _selectedStat, value))
            {
                OnPropertyChanged(nameof(PreviewText));
            }
        }
    }

    public ActionOption? SelectedOperator
    {
        get => _selectedOperator;
        set
        {
            if (SetProperty(ref _selectedOperator, value))
            {
                OnPropertyChanged(nameof(PreviewText));
            }
        }
    }

    public string ValueText
    {
        get => _valueText;
        set
        {
            if (SetProperty(ref _valueText, value))
            {
                OnPropertyChanged(nameof(PreviewText));
            }
        }
    }

    public string CustomToken
    {
        get => _customToken;
        set
        {
            if (SetProperty(ref _customToken, value))
            {
                OnPropertyChanged(nameof(PreviewText));
            }
        }
    }

    public bool ShowsStat => SelectedType?.Value is DialogActions.IncreaseStat or DialogActions.CheckStat;

    public bool ShowsOperator => SelectedType?.Value == DialogActions.CheckStat;

    public bool ShowsValue => SelectedType?.Value is DialogActions.IncreaseStat or DialogActions.CheckStat;

    public bool ShowsCustomToken => SelectedType?.Value == DialogActions.Custom;

    public string PreviewText => BuildToken();

    public string BuildToken()
    {
        var type = SelectedType?.Value ?? DialogActions.Custom;
        return type switch
        {
            DialogActions.IncreaseStat => $"{DialogActions.IncreaseStat}:{SelectedStat?.Value ?? "strength"}:{NormalizeInteger(ValueText, 1)}",
            DialogActions.CheckStat => $"{DialogActions.CheckStat}:{SelectedStat?.Value ?? "strength"}:{SelectedOperator?.Value ?? ">="}:{NormalizeInteger(ValueText, 1)}",
            DialogActions.OpenDoor => DialogActions.OpenDoor,
            DialogActions.EndDialog => DialogActions.EndDialog,
            DialogActions.Custom => string.IsNullOrWhiteSpace(CustomToken) ? "custom_token" : CustomToken.Trim(),
            _ => type,
        };
    }

    public static ActionClauseViewModel FromToken(string token)
    {
        var clause = new ActionClauseViewModel();
        token = DialogActions.CanonicalizeToken(token);

        if (string.Equals(token, DialogActions.OpenDoor, StringComparison.OrdinalIgnoreCase))
        {
            clause.SelectedType = FindOption(clause.ClauseTypes, DialogActions.OpenDoor);
            return clause;
        }

        if (string.Equals(token, DialogActions.EndDialog, StringComparison.OrdinalIgnoreCase))
        {
            clause.SelectedType = FindOption(clause.ClauseTypes, DialogActions.EndDialog);
            return clause;
        }

        var increaseParts = SplitAndTrim(token, ':');
        if (increaseParts.Length == 3 && string.Equals(increaseParts[0], DialogActions.IncreaseStat, StringComparison.OrdinalIgnoreCase))
        {
            clause.SelectedType = FindOption(clause.ClauseTypes, DialogActions.IncreaseStat);
            clause.SelectedStat = FindOption(clause.StatOptions, increaseParts[1]) ?? clause.StatOptions[0];
            clause.ValueText = increaseParts[2];
            return clause;
        }

        var checkParts = SplitAndTrim(token, ':');
        if (checkParts.Length == 4 && string.Equals(checkParts[0], DialogActions.CheckStat, StringComparison.OrdinalIgnoreCase))
        {
            clause.SelectedType = FindOption(clause.ClauseTypes, DialogActions.CheckStat);
            clause.SelectedStat = FindOption(clause.StatOptions, checkParts[1]) ?? clause.StatOptions[0];
            clause.SelectedOperator = FindOption(clause.OperatorOptions, checkParts[2]) ?? clause.OperatorOptions[0];
            clause.ValueText = checkParts[3];
            return clause;
        }

        clause.SelectedType = FindOption(clause.ClauseTypes, DialogActions.Custom);
        clause.CustomToken = token;
        return clause;
    }

    private static int NormalizeInteger(string? rawValue, int fallback)
    {
        return int.TryParse(rawValue, out var parsed) ? parsed : fallback;
    }

    private static ActionOption? FindOption(IEnumerable<ActionOption> options, string value)
    {
        foreach (var option in options)
        {
            if (string.Equals(option.Value, value, StringComparison.OrdinalIgnoreCase))
            {
                return option;
            }
        }

        return null;
    }

    private static string[] SplitAndTrim(string value, char separator)
    {
        var parts = value.Split(separator);
        for (var i = 0; i < parts.Length; i++)
        {
            parts[i] = parts[i].Trim();
        }

        return parts;
    }
}
