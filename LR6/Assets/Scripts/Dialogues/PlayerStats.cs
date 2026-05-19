using UnityEngine;

namespace LR6.Dialogues
{
    public sealed class PlayerStats : MonoBehaviour
    {
        [SerializeField] private int strength = 1;
        [SerializeField] private int intelligence = 1;
        [SerializeField] private int charisma = 1;

        public int Strength => strength;

        public int Intelligence => intelligence;

        public int Charisma => charisma;

        public int GetStat(string statName)
        {
            return statName.ToLowerInvariant() switch
            {
                "strength" or "power" => strength,
                "intelligence" or "intellect" => intelligence,
                "charisma" => charisma,
                _ => 0,
            };
        }

        public void IncreaseStat(string statName, int amount)
        {
            switch (statName.ToLowerInvariant())
            {
                case "strength":
                case "power":
                    strength += amount;
                    break;

                case "intelligence":
                case "intellect":
                    intelligence += amount;
                    break;

                case "charisma":
                    charisma += amount;
                    break;

                default:
                    Debug.LogWarning($"Unknown stat '{statName}'.");
                    break;
            }
        }
    }
}
