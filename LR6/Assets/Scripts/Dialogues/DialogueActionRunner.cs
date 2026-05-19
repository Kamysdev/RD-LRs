using System;
using System.Collections.Generic;
using UnityEngine;

namespace LR6.Dialogues
{
    public sealed class DialogueActionRunner
    {
        private readonly Dictionary<string, Action<DialogueRuntimeContext>> customActions = new();

        public void SetAction(string name, Action<DialogueRuntimeContext> action)
        {
            customActions[name.ToLowerInvariant()] = action;
        }

        public bool IsAnswerAvailable(DialogueAnswer answer, DialogueRuntimeContext context)
        {
            foreach (var token in SplitActions(answer.Action))
            {
                if (TryParseCondition(token, out var statName, out var operation, out var expectedValue) &&
                    !EvaluateCondition(context.PlayerStats, statName, operation, expectedValue))
                {
                    return false;
                }
            }

            return true;
        }

        public bool Execute(DialogueAnswer answer, DialogueRuntimeContext context)
        {
            var shouldEndDialogue = false;

            foreach (var token in SplitActions(answer.Action))
            {
                if (TryParseCondition(token, out _, out _, out _))
                {
                    continue;
                }

                if (TryParseIncrease(token, out var statName, out var amount))
                {
                    context.PlayerStats?.IncreaseStat(statName, amount);
                    continue;
                }

                if (IsToken(token, "dialogue end") || IsToken(token, "end_dialog"))
                {
                    shouldEndDialogue = true;
                    continue;
                }

                if (IsToken(token, "door open") || IsToken(token, "open_door"))
                {
                    context.Interactable?.OpenDoor();
                    continue;
                }

                if (IsToken(token, "none"))
                {
                    continue;
                }

                if (customActions.TryGetValue(token.ToLowerInvariant(), out var customAction))
                {
                    customAction(context);
                    continue;
                }

                Debug.LogWarning($"Unknown dialogue action token '{token}'.");
            }

            return shouldEndDialogue;
        }

        private static IEnumerable<string> SplitActions(string rawAction)
        {
            if (string.IsNullOrWhiteSpace(rawAction))
            {
                yield return "none";
                yield break;
            }

            foreach (var rawPart in rawAction.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var part = rawPart.Trim();
                if (part.Length > 0)
                {
                    yield return part;
                }
            }
        }

        private static bool TryParseIncrease(string token, out string statName, out int amount)
        {
            statName = string.Empty;
            amount = 0;

            var parts = SplitAndTrim(token, ':');
            if (parts.Length == 3 && parts[0].Equals("stat_inc", StringComparison.OrdinalIgnoreCase))
            {
                statName = parts[1];
                return int.TryParse(parts[2], out amount);
            }

            return false;
        }

        private static bool TryParseCondition(string token, out string statName, out string operation, out int expectedValue)
        {
            statName = string.Empty;
            operation = string.Empty;
            expectedValue = 0;

            var parts = SplitAndTrim(token, ':');
            if (parts.Length == 4 && parts[0].Equals("stat_check", StringComparison.OrdinalIgnoreCase))
            {
                statName = parts[1];
                operation = parts[2];
                return int.TryParse(parts[3], out expectedValue);
            }

            return false;
        }

        private static bool EvaluateCondition(PlayerStats? stats, string statName, string operation, int expectedValue)
        {
            if (stats == null)
            {
                return false;
            }

            var currentValue = stats.GetStat(statName);
            return operation switch
            {
                ">" => currentValue > expectedValue,
                ">=" => currentValue >= expectedValue,
                "<" => currentValue < expectedValue,
                "<=" => currentValue <= expectedValue,
                "==" or "=" => currentValue == expectedValue,
                "!=" => currentValue != expectedValue,
                _ => false,
            };
        }

        private static bool IsToken(string token, string value)
        {
            return token.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        private static string[] SplitAndTrim(string value, char separator)
        {
            var parts = value.Split(new[] { separator }, StringSplitOptions.None);
            for (var i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();
            }

            return parts;
        }
    }
}
