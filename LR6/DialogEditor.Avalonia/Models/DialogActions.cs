using System;
using System.Collections.Generic;

namespace DialogEditor.Avalonia.Models;

public static class DialogActions
{
    public const string None = "none";
    public const string OpenDoor = "door open";
    public const string EndDialog = "dialogue end";
    public const string IncreaseStat = "stat_inc";
    public const string CheckStat = "stat_check";
    public const string Custom = "custom";

    public static IReadOnlyList<ActionOption> ClauseTypes { get; } =
    [
        new ActionOption(IncreaseStat, "Increase stat"),
        new ActionOption(CheckStat, "Check stat"),
        new ActionOption(OpenDoor, "Open door"),
        new ActionOption(EndDialog, "End dialogue"),
        new ActionOption(Custom, "Custom token"),
    ];

    public static IReadOnlyList<ActionOption> Stats { get; } =
    [
        new ActionOption("strength", "Strength"),
        new ActionOption("intelligence", "Intelligence"),
        new ActionOption("charisma", "Charisma"),
    ];

    public static IReadOnlyList<ActionOption> CompareOperators { get; } =
    [
        new ActionOption(">=", "Greater or equal"),
        new ActionOption(">", "Greater"),
        new ActionOption("==", "Equal"),
        new ActionOption("!=", "Not equal"),
        new ActionOption("<=", "Less or equal"),
        new ActionOption("<", "Less"),
    ];

    public static string GetDisplayName(string action)
    {
        action = CanonicalizeActionString(action);

        return action switch
        {
            None => "No action",
            OpenDoor => "Open door",
            EndDialog => "End dialogue",
            _ when action.StartsWith(IncreaseStat + ":", StringComparison.OrdinalIgnoreCase) => "Increase stat",
            _ when action.StartsWith(CheckStat + ":", StringComparison.OrdinalIgnoreCase) => "Check stat",
            _ => action,
        };
    }

    public static string CanonicalizeActionString(string? action)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            return None;
        }

        var parts = action.Split(';', StringSplitOptions.RemoveEmptyEntries);
        var normalized = new List<string>();

        foreach (var rawPart in parts)
        {
            var token = CanonicalizeToken(rawPart);
            if (!string.IsNullOrWhiteSpace(token) && !string.Equals(token, None, StringComparison.OrdinalIgnoreCase))
            {
                normalized.Add(token);
            }
        }

        return normalized.Count == 0 ? None : string.Join("; ", normalized);
    }

    public static string CanonicalizeToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return None;
        }

        token = token.Trim();

        if (string.Equals(token, "open_door", StringComparison.OrdinalIgnoreCase))
        {
            return OpenDoor;
        }

        if (string.Equals(token, "end_dialog", StringComparison.OrdinalIgnoreCase))
        {
            return EndDialog;
        }

        if (string.Equals(token, None, StringComparison.OrdinalIgnoreCase))
        {
            return None;
        }

        return token;
    }
}
