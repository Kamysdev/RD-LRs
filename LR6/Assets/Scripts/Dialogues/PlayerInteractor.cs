using UnityEngine;

namespace LR6.Dialogues
{
    public sealed class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private LayerMask npcLayerMask;
        [SerializeField] private Camera mainCamera = null!;
        [SerializeField] private float interactionDistance = 100f;

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0) || mainCamera == null)
            {
                return;
            }

            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, interactionDistance, npcLayerMask))
            {
                var interactable = hit.transform.GetComponent<DialogueInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
}
