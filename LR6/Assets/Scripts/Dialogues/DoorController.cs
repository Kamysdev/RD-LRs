using UnityEngine;

namespace LR6.Dialogues
{
    public sealed class DoorController : MonoBehaviour
    {
        [SerializeField] private Animator animator = null!;
        [SerializeField] private string openParameter = "isOpen";

        public void OpenDoor()
        {
            if (animator != null)
            {
                animator.SetBool(openParameter, true);
            }
        }

        public void CloseDoor()
        {
            if (animator != null)
            {
                animator.SetBool(openParameter, false);
            }
        }
    }
}
