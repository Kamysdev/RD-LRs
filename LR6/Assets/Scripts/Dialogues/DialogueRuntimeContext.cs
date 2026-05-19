namespace LR6.Dialogues
{
    public sealed class DialogueRuntimeContext
    {
        public DialogueRuntimeContext(PlayerStats? playerStats, DialogueInteractable? interactable)
        {
            PlayerStats = playerStats;
            Interactable = interactable;
        }

        public PlayerStats? PlayerStats { get; }

        public DialogueInteractable? Interactable { get; }
    }
}
