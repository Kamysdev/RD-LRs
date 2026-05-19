using UnityEngine;

namespace LR6.Dialogues
{
    public sealed class DialogueInteractable : MonoBehaviour
    {
        [SerializeField] private TextAsset dialogueAsset = null!;
        [SerializeField] private DialogueSystem dialogueSystem = null!;
        [SerializeField] private DoorController door = null!;

        public void Interact()
        {
            if (dialogueAsset == null || dialogueSystem == null)
            {
                Debug.LogWarning("DialogueInteractable is missing references.");
                return;
            }

            dialogueSystem.StartDialogue(dialogueAsset, this);
        }

        public void OpenDoor()
        {
            if (door != null)
            {
                door.OpenDoor();
            }
        }
    }
}
